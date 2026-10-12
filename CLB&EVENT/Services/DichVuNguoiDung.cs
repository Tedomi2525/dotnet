using System;
using System.Collections.Generic;
using System.Linq;
using CLB_EVENT.Models;
using CLB_EVENT.DuLieu;

namespace CLB_EVENT.Services
{
    /// <summary>
    /// Dịch vụ quản lý người dùng: đăng ký, đăng nhập, tra cứu
    /// Snapshot làm việc được tải và lưu bằng KhoDuLieuAiven.
    /// </summary>
    public class DichVuNguoiDung
    {
        // Snapshot người dùng; dữ liệu gốc nằm trong MySQL.
        private static List<NguoiDung> _danhSachNguoiDung = new List<NguoiDung>();
        internal static List<NguoiDung> DuLieuNguoiDung => _danhSachNguoiDung;
        internal static void DongBoPhienDangNhap()
        {
            if (NguoiDungHienTai == null) return;
            var ma = NguoiDungHienTai.MaNguoiDung;
            NguoiDungHienTai = _danhSachNguoiDung.FirstOrDefault(n => n.MaNguoiDung == ma && n.TrangThaiHoatDong);
        }

        // Người dùng hiện đang đăng nhập
        public static NguoiDung NguoiDungHienTai { get; private set; }

        /// <summary>
        /// Khởi tạo dữ liệu mẫu khi ứng dụng bắt đầu
        /// </summary>
        static DichVuNguoiDung()
        {
            KhoiTaoDuLieuMau();
        }

        /// <summary>
        /// Tạo dữ liệu người dùng mẫu ban đầu
        /// </summary>
        private static void KhoiTaoDuLieuMau()
        {
            // Tài khoản Admin
            var admin = new NguoiDung
            {
                MaNguoiDung = "ND001",
                HoTen = "Nguyễn Văn Quản Trị",
                TenDangNhap = "admin",
                MatKhauMaHoa = DichVuMaHoa.MaHoaSHA256("Admin@123"),
                Email = "admin@truong.edu.vn",
                SoDienThoai = "0901234567",
                VaiTro = "QuanLyAdmin",
                LopHoc = "Ban Quản Lý"
            };

            // Tài khoản sinh viên mẫu
            var sinhVien1 = new NguoiDung
            {
                MaNguoiDung = "ND002",
                HoTen = "Trần Thị Bình An",
                TenDangNhap = "binghan",
                MatKhauMaHoa = DichVuMaHoa.MaHoaSHA256("SinhVien@123"),
                Email = "binhan@sinhvien.edu.vn",
                SoDienThoai = "0912345678",
                VaiTro = "SinhVien",
                LopHoc = "CNTT-K20"
            };

            // Tài khoản quản lý CLB
            var quanLyCLB = new NguoiDung
            {
                MaNguoiDung = "ND003",
                HoTen = "Lê Minh Tuấn",
                TenDangNhap = "tuanclb",
                MatKhauMaHoa = DichVuMaHoa.MaHoaSHA256("QuanLy@123"),
                Email = "tuanclb@truong.edu.vn",
                SoDienThoai = "0923456789",
                VaiTro = "QuanLyCLB",
                LopHoc = "QTKD-K19"
            };

            _danhSachNguoiDung.AddRange(new[] { admin, sinhVien1, quanLyCLB });
        }

        /// <summary>
        /// Xử lý đăng nhập: kiểm tra tên đăng nhập và mật khẩu
        /// </summary>
        /// <param name="tenDangNhap">Tên đăng nhập</param>
        /// <param name="matKhau">Mật khẩu gốc (chưa mã hóa)</param>
        /// <returns>Người dùng nếu đăng nhập thành công, null nếu thất bại</returns>
        public NguoiDung DangNhap(string tenDangNhap, string matKhau)
        {
            return KhoDuLieuAiven.Doc(() =>
            {
                if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
                    return null;
                // Tìm người dùng theo tên đăng nhập
                var nguoiDung = _danhSachNguoiDung
                    .FirstOrDefault(nd => nd.TenDangNhap.Equals(tenDangNhap, StringComparison.OrdinalIgnoreCase));
                if (nguoiDung == null)
                    return null;
                // Kiểm tra tài khoản có bị khóa không
                if (!nguoiDung.TrangThaiHoatDong)
                    return null;
                // Xác thực mật khẩu bằng hàm băm SHA-256
                bool matKhauDung = DichVuMaHoa.XacThucMatKhau(matKhau, nguoiDung.MatKhauMaHoa);
                if (!matKhauDung)
                    return null;
                // Lưu người dùng hiện tại vào session
                NguoiDungHienTai = nguoiDung;
                return nguoiDung;
            });
        }

        /// <summary>
        /// Đăng xuất khỏi hệ thống
        /// </summary>
        public void DangXuat()
        {
            NguoiDungHienTai = null;
        }

        /// <summary>
        /// Đăng ký tài khoản mới cho sinh viên
        /// </summary>
        /// <param name="nguoiDungMoi">Thông tin người dùng mới</param>
        /// <param name="matKhauGoc">Mật khẩu gốc (sẽ được mã hóa)</param>
        /// <returns>True nếu đăng ký thành công</returns>
        public (bool ThanhCong, string ThongBao) DangKyTaiKhoan(NguoiDung nguoiDungMoi, string matKhauGoc)
        {
            return KhoDuLieuAiven.Ghi(() =>
            {
                if (nguoiDungMoi == null || string.IsNullOrWhiteSpace(nguoiDungMoi.HoTen) ||
                    string.IsNullOrWhiteSpace(nguoiDungMoi.TenDangNhap))
                    return (false, "Vui lòng nhập họ tên và tên đăng nhập.");
                nguoiDungMoi.HoTen = nguoiDungMoi.HoTen.Trim();
                nguoiDungMoi.TenDangNhap = nguoiDungMoi.TenDangNhap.Trim();
                nguoiDungMoi.Email = nguoiDungMoi.Email?.Trim();
                if (!EmailHopLe(nguoiDungMoi.Email))
                    return (false, "Email không đúng định dạng.");
                if (!SoDienThoaiHopLe(nguoiDungMoi.SoDienThoai))
                    return (false, "Số điện thoại cần có 10 đến 15 chữ số.");
                if (!DichVuMaHoa.KiemTraDoManhMatKhau(matKhauGoc))
                    return (false, "Mật khẩu cần ít nhất 8 ký tự, có chữ hoa, chữ thường và số.");
                // Kiểm tra tên đăng nhập đã tồn tại chưa
                bool trungTen = _danhSachNguoiDung
                    .Any(nd => nd.TenDangNhap.Equals(nguoiDungMoi.TenDangNhap, StringComparison.OrdinalIgnoreCase));
                if (trungTen)
                    return (false, "Tên đăng nhập đã được sử dụng. Vui lòng chọn tên khác.");
                // Kiểm tra email đã tồn tại chưa
                bool trungEmail = _danhSachNguoiDung
                    .Any(nd => nd.Email.Equals(nguoiDungMoi.Email, StringComparison.OrdinalIgnoreCase));
                if (trungEmail)
                    return (false, "Email đã được đăng ký. Vui lòng dùng email khác.");
                // Mã hóa mật khẩu bằng SHA-256 trước khi lưu
                nguoiDungMoi.MatKhauMaHoa = DichVuMaHoa.MaHoaSHA256(matKhauGoc);
                nguoiDungMoi.NgayTao = DateTime.Now;
                nguoiDungMoi.VaiTro = "SinhVien";
                nguoiDungMoi.TrangThaiHoatDong = true;
                _danhSachNguoiDung.Add(nguoiDungMoi);
                return (true, "Đăng ký tài khoản thành công!");
            });
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng (dành cho Admin)
        /// </summary>
        public List<NguoiDung> LayDanhSachNguoiDung() => KhoDuLieuAiven.Doc(() => new List<NguoiDung>(_danhSachNguoiDung));

        /// <summary>
        /// Tìm người dùng theo mã
        /// </summary>
        public NguoiDung TimNguoiDungTheoMa(string ma) => KhoDuLieuAiven.Doc(() => _danhSachNguoiDung.FirstOrDefault(nd => nd.MaNguoiDung == ma));

        public static bool EmailHopLe(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email && address.Host.Contains(".");
            }
            catch (FormatException) { return false; }
        }

        public static bool SoDienThoaiHopLe(string soDienThoai) =>
            string.IsNullOrWhiteSpace(soDienThoai) ||
            System.Text.RegularExpressions.Regex.IsMatch(soDienThoai.Trim(), @"^\+?[0-9]{10,15}$");
    }
}
