using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using CLB_EVENT.Models;
using CLB_EVENT.Services;
using MySqlConnector;

namespace CLB_EVENT.DuLieu
{
    // Một giao dịch chứa toàn bộ thay đổi của một thao tác nghiệp vụ.
    // Khóa hàng metadata bảo vệ cả đăng ký, số chỗ và xét duyệt giữa nhiều máy.
    public static class KhoDuLieuAiven
    {
        private static readonly object DongBo = new object();
        [ThreadStatic] private static int _doSau;
        [ThreadStatic] private static bool _dangGhi;
        private static bool _sanSang;
        private static bool _kiemThuBoNho;
        private static string _chuoiKetNoi;
        private static Bang[] _bang;

        private sealed class Bang
        {
            internal readonly string Ten;
            internal readonly IList DanhSach;
            internal readonly Type Kieu;
            internal readonly PropertyInfo[] Cot;
            internal Bang(string ten, IList danhSach, Type kieu, string cacCot)
            {
                Ten = ten; DanhSach = danhSach; Kieu = kieu;
                Cot = cacCot.Split(',').Select(c => kieu.GetProperty(c)).ToArray();
                if (Cot.Any(c => c == null)) throw new InvalidOperationException("Ánh xạ bảng không hợp lệ: " + ten);
            }
            internal string Ma(object row) => (string)Cot[0].GetValue(row);
            internal object[] GiaTri(object row) => Cot.Select(c => c.GetValue(row)).ToArray();
            internal Dictionary<string, object[]> Chup() => DanhSach.Cast<object>().ToDictionary(Ma, GiaTri);
            internal string SelectSql => "SELECT " + string.Join(",", Cot.Select(c => "`" + c.Name + "`")) + " FROM `" + Ten + "`";
            internal void Nap(List<object[]> rows)
            {
                var cu = DanhSach.Cast<object>().ToDictionary(Ma);
                DanhSach.Clear();
                foreach (var values in rows)
                {
                    object row;
                    if (!cu.TryGetValue((string)values[0], out row)) row = Activator.CreateInstance(Kieu);
                    for (var i = 0; i < Cot.Length; i++) Cot[i].SetValue(row, values[i]);
                    DanhSach.Add(row);
                }
            }
            internal string TaoBangSql()
            {
                var columns = Cot.Select((c, i) => "`" + c.Name + "` " +
                    (c.PropertyType == typeof(DateTime) ? "DATETIME(6) NOT NULL" :
                     c.PropertyType == typeof(int) ? "INT NOT NULL" :
                     c.PropertyType == typeof(bool) ? "TINYINT(1) NOT NULL" :
                     i == 0 ? "VARCHAR(64) NOT NULL" :
                     c.Name.StartsWith("Ma") && c.Name != "MatKhauMaHoa" ? "VARCHAR(64) NULL" :
                     c.Name == "TenDangNhap" || c.Name == "Email" || c.Name == "TenCLB" ? "VARCHAR(254) NULL" :
                     "LONGTEXT NULL")).ToList();
                columns.Add("PRIMARY KEY (`" + Cot[0].Name + "`)");
                if (Ten == "clbe_nguoi_dung")
                {
                    columns.Add("UNIQUE KEY `uq_clbe_username` (`TenDangNhap`)");
                    columns.Add("UNIQUE KEY `uq_clbe_email` (`Email`)");
                }
                if (Ten == "clbe_cau_lac_bo") columns.Add("UNIQUE KEY `uq_clbe_club_name` (`TenCLB`)");
                foreach (var c in Cot.Skip(1).Where(c => c.Name.StartsWith("Ma") && c.Name != "MatKhauMaHoa"))
                    columns.Add("KEY `idx_" + c.Name + "` (`" + c.Name + "`)");
                string Fk(string col, string parent, string pk) => "FOREIGN KEY (`" + col + "`) REFERENCES `" + parent + "` (`" + pk + "`)";
                if (Ten == "clbe_cau_lac_bo") columns.Add(Fk("MaNguoiDungQuanLy", "clbe_nguoi_dung", "MaNguoiDung"));
                if (Ten == "clbe_su_kien" || Ten == "clbe_tin_tuc") columns.Add(Fk("MaCLB", "clbe_cau_lac_bo", "MaCLB"));
                if (Ten == "clbe_dang_ky_clb" || Ten == "clbe_dang_ky_su_kien")
                {
                    columns.Add(Fk("MaNguoiDung", "clbe_nguoi_dung", "MaNguoiDung"));
                    columns.Add(Ten == "clbe_dang_ky_clb" ? Fk("MaCLB", "clbe_cau_lac_bo", "MaCLB") :
                        Fk("MaSuKien", "clbe_su_kien", "MaSuKien"));
                }
                return "CREATE TABLE IF NOT EXISTS `" + Ten + "` (" + string.Join(",", columns) +
                    ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_as_ci";
            }
        }

        private static void TaoAnhXa()
        {
            _bang = new[] {
                new Bang("clbe_nguoi_dung", DichVuNguoiDung.DuLieuNguoiDung, typeof(NguoiDung),
                    "MaNguoiDung,HoTen,TenDangNhap,MatKhauMaHoa,Email,SoDienThoai,VaiTro,LopHoc,NgayTao,TrangThaiHoatDong"),
                new Bang("clbe_cau_lac_bo", DichVuCLB.DuLieuCLB, typeof(CauLacBo),
                    "MaCLB,TenCLB,MoTa,Email,SoDienThoai,ChuTichCLB,MaNguoiDungQuanLy,SoThanhVien,PhongBan,NgayThanhLap,DiaDiemHoatDong,TrangThaiHoatDong"),
                new Bang("clbe_su_kien", DichVuSuKien.DuLieuSuKien, typeof(SuKien),
                    "MaSuKien,TenSuKien,MoTa,MaCLB,TenCLB,DiaDiem,TrangThai,LoaiSuKien,AnhSuKien,ThoiGianBatDau,ThoiGianKetThuc,NgayTao,SoLuongToiDa,SoLuongDaDangKy"),
                new Bang("clbe_dang_ky_clb", DichVuCLB.DuLieuDangKy, typeof(DangKyCLB),
                    "MaDangKy,MaNguoiDung,HoTen,MaSinhVien,LopHoc,Email,SoDienThoai,MaCLB,TenCLB,LyDo,KyNang,ThoiGianRanh,TrangThai,ThoiGianDangKy"),
                new Bang("clbe_dang_ky_su_kien", DichVuSuKien.DuLieuDangKy, typeof(DangKySuKien),
                    "MaDangKy,MaNguoiDung,HoTen,MaSuKien,TenSuKien,TrangThai,GhiChu,Email,SoDienThoai,ThoiGianDangKy"),
                new Bang("clbe_tin_tuc", DichVuTinTuc.DuLieuTinTuc, typeof(TinTuc),
                    "MaTinTuc,TieuDe,NoiDung,TomTat,TacGia,DanhMuc,MaCLB,AnhMinhHoa,TrangThai,NgayDang,LuotXem")
            };
        }

        public static void KhoiTao()
        {
            lock (DongBo)
            {
                if (_sanSang) return;
                _chuoiKetNoi = CauHinhAiven.TaoChuoiKetNoi();
                TaoAnhXa();
                using (var cn = MoKetNoi())
                {
                    // DDL tự commit trên MySQL: dùng advisory lock để hai máy không khởi tạo cùng lúc.
                    var lockName = cn.Database + ":clbe_schema_v1";
                    using (var cmd = new MySqlCommand("SELECT GET_LOCK(@name, 15)", cn))
                    {
                        cmd.Parameters.AddWithValue("@name", lockName);
                        if (Convert.ToInt32(cmd.ExecuteScalar()) != 1) throw new InvalidOperationException("Máy khác đang khởi tạo dữ liệu. Hãy thử lại.");
                    }
                    try
                    {
                        Execute(cn, null, "CREATE TABLE IF NOT EXISTS clbe_metadata (Id TINYINT NOT NULL PRIMARY KEY, Version INT NOT NULL, Seeded TINYINT(1) NOT NULL) ENGINE=InnoDB");
                        Execute(cn, null, "INSERT IGNORE INTO clbe_metadata (Id,Version,Seeded) VALUES (1,1,0)");
                        using (var version = new MySqlCommand("SELECT Version FROM clbe_metadata WHERE Id=1", cn))
                            if (Convert.ToInt32(version.ExecuteScalar()) != 1)
                                throw new InvalidOperationException("Phiên bản dữ liệu không tương thích với ứng dụng này.");
                        foreach (var bang in _bang) Execute(cn, null, bang.TaoBangSql());
                        using (var tx = cn.BeginTransaction())
                        {
                            bool seeded;
                            using (var cmd = new MySqlCommand("SELECT Seeded FROM clbe_metadata WHERE Id=1 FOR UPDATE", cn, tx))
                                seeded = Convert.ToBoolean(cmd.ExecuteScalar());
                            if (!seeded)
                            {
                                long count;
                                using (var cmd = new MySqlCommand("SELECT " + string.Join("+", _bang.Select(b => "(SELECT COUNT(*) FROM `" + b.Ten + "`)")), cn, tx))
                                    count = Convert.ToInt64(cmd.ExecuteScalar());
                                // Chỉ đưa dữ liệu mẫu vào bộ bảng hoàn toàn trống, một lần duy nhất.
                                if (count == 0)
                                    foreach (var bang in _bang)
                                        foreach (var row in bang.DanhSach) LuuHang(cn, tx, bang, row, true);
                                Execute(cn, tx, "UPDATE clbe_metadata SET Seeded=1 WHERE Id=1");
                            }
                            tx.Commit();
                        }
                    }
                    finally
                    {
                        using (var cmd = new MySqlCommand("SELECT RELEASE_LOCK(@name)", cn))
                        { cmd.Parameters.AddWithValue("@name", lockName); cmd.ExecuteScalar(); }
                    }
                    using (var tx = cn.BeginTransaction(IsolationLevel.RepeatableRead))
                    { NapDuLieu(cn, tx); tx.Commit(); }
                }
                _sanSang = true;
            }
        }

        // Chỉ test harness gọi; ứng dụng thật luôn khởi tạo Aiven và không tự rơi về RAM.
        public static void DungBoNhoChoKiemThu()
        {
            lock (DongBo)
            {
                if (_sanSang) throw new InvalidOperationException("Đã khởi tạo Aiven.");
                _kiemThuBoNho = true;
            }
        }

        private static void KiemTraSanSang()
        {
            if (!_sanSang && !_kiemThuBoNho) throw new InvalidOperationException("Chưa khởi tạo kết nối Aiven.");
        }

        public static T Doc<T>(Func<T> thaoTac)
        {
            lock (DongBo)
            {
                KiemTraSanSang();
                if (_kiemThuBoNho || _doSau > 0) return thaoTac();
                _doSau++;
                try
                {
                    using (var cn = MoKetNoi())
                    using (var tx = cn.BeginTransaction(IsolationLevel.RepeatableRead))
                    { NapDuLieu(cn, tx); tx.Commit(); }
                    return thaoTac();
                }
                finally { _doSau--; }
            }
        }

        public static T Ghi<T>(Func<T> thaoTac)
        {
            lock (DongBo)
            {
                KiemTraSanSang();
                if (_kiemThuBoNho) return thaoTac();
                if (_doSau > 0)
                {
                    if (!_dangGhi) throw new InvalidOperationException("Không được ghi dữ liệu từ thao tác chỉ đọc.");
                    return thaoTac();
                }
                _doSau++;
                _dangGhi = true;
                Dictionary<string, object[]>[] truoc = null;
                try
                {
                    using (var cn = MoKetNoi())
                    using (var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        Execute(cn, tx, "SELECT Id FROM clbe_metadata WHERE Id=1 FOR UPDATE");
                        NapDuLieu(cn, tx);
                        truoc = _bang.Select(b => b.Chup()).ToArray();
                        var result = thaoTac();
                        if ((result is ValueTuple<bool, string> && !((ValueTuple<bool, string>)(object)result).Item1) ||
                            (result is bool && !(bool)(object)result))
                        {
                            tx.Rollback(); KhoiPhuc(truoc); return result;
                        }
                        for (var i = 0; i < _bang.Length; i++)
                        {
                            var bang = _bang[i];
                            if (truoc[i].Keys.Except(bang.DanhSach.Cast<object>().Select(bang.Ma)).Any())
                                throw new InvalidOperationException("Không được xóa lịch sử dữ liệu.");
                            foreach (var row in bang.DanhSach)
                            {
                                object[] values;
                                bool moi = !truoc[i].TryGetValue(bang.Ma(row), out values);
                                if (moi || !values.SequenceEqual(bang.GiaTri(row))) LuuHang(cn, tx, bang, row, moi);
                            }
                        }
                        tx.Commit();
                        return result;
                    }
                }
                catch
                {
                    if (truoc != null) KhoiPhuc(truoc);
                    throw;
                }
                finally { _dangGhi = false; _doSau--; }
            }
        }

        private static MySqlConnection MoKetNoi()
        {
            var cn = new MySqlConnection(_chuoiKetNoi);
            try { cn.Open(); return cn; }
            catch { cn.Dispose(); throw; }
        }
        private static void Execute(MySqlConnection cn, MySqlTransaction tx, string sql)
        { using (var cmd = new MySqlCommand(sql, cn, tx)) cmd.ExecuteNonQuery(); }

        private static void NapDuLieu(MySqlConnection cn, MySqlTransaction tx)
        {
            // Một lượt gửi SQL lấy snapshot nhất quán của sáu bảng.
            var data = new List<object[]>[_bang.Length];
            using (var cmd = new MySqlCommand(string.Join(";", _bang.Select(b => b.SelectSql)), cn, tx))
            using (var reader = cmd.ExecuteReader())
            {
                for (var b = 0; b < _bang.Length; b++)
                {
                    data[b] = new List<object[]>();
                    while (reader.Read())
                    {
                        var values = new object[_bang[b].Cot.Length];
                        for (var i = 0; i < values.Length; i++)
                            values[i] = reader.IsDBNull(i) ? null : Convert.ChangeType(reader.GetValue(i), _bang[b].Cot[i].PropertyType);
                        data[b].Add(values);
                    }
                    if (b < _bang.Length - 1 && !reader.NextResult()) throw new InvalidOperationException("Dữ liệu Aiven chưa đầy đủ.");
                }
            }
            for (var i = 0; i < _bang.Length; i++) _bang[i].Nap(data[i]);
            DichVuNguoiDung.DongBoPhienDangNhap();
        }

        private static void KhoiPhuc(Dictionary<string, object[]>[] data)
        {
            for (var i = 0; i < _bang.Length; i++) _bang[i].Nap(data[i].Values.ToList());
            DichVuNguoiDung.DongBoPhienDangNhap();
        }

        private static void LuuHang(MySqlConnection cn, MySqlTransaction tx, Bang bang, object row, bool moi)
        {
            string Cot(int i) => "`" + bang.Cot[i].Name + "`";
            var sql = moi ? "INSERT INTO `" + bang.Ten + "` (" + string.Join(",", Enumerable.Range(0, bang.Cot.Length).Select(Cot)) +
                ") VALUES (" + string.Join(",", Enumerable.Range(0, bang.Cot.Length).Select(i => "@p" + i)) + ")" :
                "UPDATE `" + bang.Ten + "` SET " + string.Join(",", Enumerable.Range(1, bang.Cot.Length - 1).Select(i => Cot(i) + "=@p" + i)) +
                " WHERE " + Cot(0) + "=@p0";
            using (var cmd = new MySqlCommand(sql, cn, tx))
            {
                for (var i = 0; i < bang.Cot.Length; i++)
                {
                    var value = bang.Cot[i].GetValue(row);
                    if (i > 0 && bang.Cot[i].Name.StartsWith("Ma") && value is string && string.IsNullOrWhiteSpace((string)value)) value = null;
                    cmd.Parameters.AddWithValue("@p" + i, value ?? DBNull.Value);
                }
                cmd.ExecuteNonQuery();
            }
        }

        public static string ThongBaoLoi(Exception ex)
        {
            while (ex is TargetInvocationException && ex.InnerException != null) ex = ex.InnerException;
            var mysql = ex as MySqlException;
            if (mysql != null)
            {
                if (mysql.Number == 1045) return "Aiven từ chối đăng nhập. Cập nhật mật khẩu bằng scripts/Set-AivenPassword.ps1 rồi mở lại ứng dụng.";
                if (mysql.Number == 1062) return "Thông tin này đã tồn tại trên Aiven. Hãy kiểm tra tên đăng nhập, email hoặc tên CLB.";
                return "Không thể hoàn tất thao tác với Aiven (mã " + mysql.Number + "). Kiểm tra mạng và trạng thái dịch vụ, rồi tải lại dữ liệu trước khi thử lại.";
            }
            if (ex is InvalidOperationException) return ex.Message;
            return "Không thể kết nối dữ liệu Aiven. Kiểm tra cấu hình và chứng chỉ ca.pem.";
        }
    }
}
