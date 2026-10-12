using System;
using System.Collections.Generic;
using System.Linq;
using CLB_EVENT.Models;
using CLB_EVENT.DuLieu;

namespace CLB_EVENT.Services
{
    /// <summary>
    /// Dịch vụ quản lý Sự Kiện: tạo, tra cứu, đăng ký tham gia sự kiện
    /// </summary>
    public class DichVuSuKien
    {
        // Danh sách sự kiện lưu trong bộ nhớ
        private static List<SuKien> _danhSachSuKien = new List<SuKien>();
        internal static List<SuKien> DuLieuSuKien => _danhSachSuKien;
        internal static List<DangKySuKien> DuLieuDangKy => _danhSachDangKy;

        // Danh sách đăng ký sự kiện
        private static List<DangKySuKien> _danhSachDangKy = new List<DangKySuKien>();

        /// <summary>
        /// Khởi tạo dữ liệu mẫu khi tải lên
        /// </summary>
        static DichVuSuKien()
        {
            KhoiTaoDuLieuMau();
        }

        /// <summary>
        /// Tạo dữ liệu sự kiện mẫu
        /// </summary>
        private static void KhoiTaoDuLieuMau()
        {
            _danhSachSuKien.AddRange(new[]
            {
                new SuKien
                {
                    MaSuKien = "SK001",
                    TenSuKien = "Hackathon Lập Trình 2026",
                    MoTa = "Cuộc thi lập trình 24 giờ dành cho sinh viên yêu thích công nghệ. " +
                           "Các đội thi gồm 3-4 sinh viên sẽ giải quyết các bài toán thực tế trong 24 giờ liên tục.",
                    MaCLB = "CLB001",
                    TenCLB = "CLB Công Nghệ Thông Tin",
                    DiaDiem = "Hội trường A - Tòa nhà Trung Tâm",
                    ThoiGianBatDau = DateTime.Now.AddDays(7),
                    ThoiGianKetThuc = DateTime.Now.AddDays(8),
                    SoLuongToiDa = 100,
                    SoLuongDaDangKy = 45,
                    TrangThai = "SắpDiễnRa",
                    LoaiSuKien = "HọcThuật"
                },
                new SuKien
                {
                    MaSuKien = "SK002",
                    TenSuKien = "Đêm Văn Nghệ Mùa Thu 2026",
                    MoTa = "Chương trình văn nghệ đặc sắc chào đón mùa thu với các tiết mục ca múa nhạc kịch. " +
                           "Cơ hội để sinh viên thể hiện tài năng nghệ thuật.",
                    MaCLB = "CLB003",
                    TenCLB = "CLB Văn Hóa Nghệ Thuật",
                    DiaDiem = "Sân khấu ngoài trời - Khu vực D",
                    ThoiGianBatDau = DateTime.Now.AddDays(14),
                    ThoiGianKetThuc = DateTime.Now.AddDays(14).AddHours(4),
                    SoLuongToiDa = 500,
                    SoLuongDaDangKy = 320,
                    TrangThai = "SắpDiễnRa",
                    LoaiSuKien = "VănHóaNghệThuật"
                },
                new SuKien
                {
                    MaSuKien = "SK003",
                    TenSuKien = "Giải Bóng Đá Sinh Viên 2026",
                    MoTa = "Giải bóng đá 7 người dành cho sinh viên các khoa. " +
                           "Mỗi khoa cử 1 đội tham dự, giải thưởng hấp dẫn cho đội vô địch.",
                    MaCLB = "CLB004",
                    TenCLB = "CLB Thể Thao",
                    DiaDiem = "Sân bóng đá - Khu thể thao",
                    ThoiGianBatDau = DateTime.Now.AddDays(21),
                    ThoiGianKetThuc = DateTime.Now.AddDays(25),
                    SoLuongToiDa = 200,
                    SoLuongDaDangKy = 88,
                    TrangThai = "SắpDiễnRa",
                    LoaiSuKien = "ThểThao"
                },
                new SuKien
                {
                    MaSuKien = "SK004",
                    TenSuKien = "Workshop: Kỹ Năng Thuyết Trình Tiếng Anh",
                    MoTa = "Workshop 1 ngày giúp sinh viên nâng cao kỹ năng thuyết trình bằng tiếng Anh. " +
                           "Chuyên gia người nước ngoài trực tiếp hướng dẫn.",
                    MaCLB = "CLB005",
                    TenCLB = "CLB Tiếng Anh",
                    DiaDiem = "Phòng hội thảo 201 - Tòa B",
                    ThoiGianBatDau = DateTime.Now.AddDays(5),
                    ThoiGianKetThuc = DateTime.Now.AddDays(5).AddHours(8),
                    SoLuongToiDa = 50,
                    SoLuongDaDangKy = 50,
                    TrangThai = "SắpDiễnRa",
                    LoaiSuKien = "HộiThảo"
                },
                new SuKien
                {
                    MaSuKien = "SK005",
                    TenSuKien = "Ngày Hội Khởi Nghiệp 2026",
                    MoTa = "Sự kiện kết nối sinh viên với các doanh nhân, nhà đầu tư. " +
                           "Trình bày ý tưởng khởi nghiệp và nhận tư vấn từ chuyên gia.",
                    MaCLB = "CLB006",
                    TenCLB = "CLB Khởi Nghiệp",
                    DiaDiem = "Hội trường B - Tòa nhà Trung Tâm",
                    ThoiGianBatDau = DateTime.Now.AddDays(30),
                    ThoiGianKetThuc = DateTime.Now.AddDays(30).AddHours(6),
                    SoLuongToiDa = 150,
                    SoLuongDaDangKy = 62,
                    TrangThai = "SắpDiễnRa",
                    LoaiSuKien = "HộiThảo"
                },
                new SuKien
                {
                    MaSuKien = "SK006",
                    TenSuKien = "Seminar: AI và Tương Lai Nghề Nghiệp",
                    MoTa = "Chuyên gia AI hàng đầu chia sẻ về tác động của trí tuệ nhân tạo đến thị trường việc làm. " +
                           "Cơ hội giao lưu và đặt câu hỏi trực tiếp.",
                    MaCLB = "CLB001",
                    TenCLB = "CLB Công Nghệ Thông Tin",
                    DiaDiem = "Phòng hội thảo lớn - Tòa A",
                    ThoiGianBatDau = DateTime.Now.AddDays(-3),
                    ThoiGianKetThuc = DateTime.Now.AddDays(-3).AddHours(3),
                    SoLuongToiDa = 200,
                    SoLuongDaDangKy = 198,
                    TrangThai = "ĐãKếtThúc",
                    LoaiSuKien = "HộiThảo"
                }
            });
        }

        private static readonly object _dongBo = new object();

        private void CapNhatTrangThai()
        {
            var now = DateTime.Now;
            foreach (var sk in _danhSachSuKien.Where(s => s.TrangThai != "ĐãHủy"))
            {
                sk.TenCLB = new DichVuCLB().TimCLBTheoMa(sk.MaCLB)?.TenCLB ?? sk.TenCLB;
                sk.TrangThai = sk.ThoiGianKetThuc <= now ? "ĐãKếtThúc" :
                    sk.ThoiGianBatDau <= now ? "ĐangDiễnRa" : "SắpDiễnRa";
            }
        }

        public List<SuKien> LayDanhSachSuKien()
        {
            return KhoDuLieuAiven.Doc(() =>
            {
                CapNhatTrangThai();
                return new List<SuKien>(_danhSachSuKien);
            });
        }

        public List<SuKien> LaySuKienSapDienRa() => KhoDuLieuAiven.Doc(() => LayDanhSachSuKien().Where(s => s.TrangThai == "SắpDiễnRa" &&
                new DichVuCLB().TimCLBTheoMa(s.MaCLB)?.TrangThaiHoatDong == true)
                .OrderBy(s => s.ThoiGianBatDau).ToList());

        public List<SuKien> LayDanhSachSuKienSapToi() => KhoDuLieuAiven.Doc(() => LaySuKienSapDienRa());

        public List<SuKien> LaySuKienDuocQuanLy() => KhoDuLieuAiven.Doc(() => LayDanhSachSuKien().Where(s => DichVuPhanQuyen.QuanLyCLB(
                new DichVuCLB().TimCLBTheoMa(s.MaCLB))).OrderBy(s => s.ThoiGianBatDau).ToList());

        public List<SuKien> TimKiemSuKien(string tuKhoa = null, string loai = null, string maCLB = null) => KhoDuLieuAiven.Doc(() => LayDanhSachSuKien().Where(s =>
                (string.IsNullOrWhiteSpace(tuKhoa) || (s.TenSuKien ?? "").IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (s.MoTa ?? "").IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0) &&
                (string.IsNullOrWhiteSpace(loai) || s.LoaiSuKien == loai) &&
                (string.IsNullOrWhiteSpace(maCLB) || s.MaCLB == maCLB)).ToList());

        public (bool ThanhCong, string ThongBao) DangKyThamGia(DangKySuKien don)
        {
            return KhoDuLieuAiven.Ghi(() =>
            {
                lock (_dongBo)
                {
                    if (don == null || !DichVuPhanQuyen.LaNguoiDungHienTai(don.MaNguoiDung))
                        return (false, "Vui lòng đăng nhập bằng tài khoản đăng ký.");
                    var sk = LaySuKienTheoMa(don.MaSuKien);
                    if (sk == null) return (false, "Sự kiện không tồn tại.");
                    var clb = new DichVuCLB().TimCLBTheoMa(sk.MaCLB);
                    if (clb == null || !clb.TrangThaiHoatDong) return (false, "CLB tổ chức không còn hoạt động.");
                    if (sk.TrangThai != "SắpDiễnRa" || sk.ThoiGianBatDau <= DateTime.Now)
                        return (false, "Sự kiện đã bắt đầu hoặc không còn nhận đăng ký.");
                    if (!sk.ConChoTrong) return (false, "Sự kiện đã đầy chỗ.");
                    if (_danhSachDangKy.Any(d => d.MaNguoiDung == don.MaNguoiDung &&
                        d.MaSuKien == don.MaSuKien && d.TrangThai != "ĐãHủy"))
                        return (false, "Bạn đã đăng ký sự kiện này.");
                    var user = DichVuNguoiDung.NguoiDungHienTai;
                    don.MaDangKy = Guid.NewGuid().ToString();
                    don.HoTen = user.HoTen;
                    don.Email = user.Email;
                    don.SoDienThoai = user.SoDienThoai;
                    don.TenSuKien = sk.TenSuKien;
                    don.TrangThai = "ĐãĐăngKý";
                    don.ThoiGianDangKy = DateTime.Now;
                    _danhSachDangKy.Add(don);
                    sk.SoLuongDaDangKy++;
                    return (true, "Đăng ký sự kiện thành công.");
                }
            });
        }

        public (bool ThanhCong, string ThongBao) DangKySuKien(string maNguoiDung, string maSuKien, string ghiChu) => KhoDuLieuAiven.Ghi(() => DangKyThamGia(new DangKySuKien { MaNguoiDung = maNguoiDung, MaSuKien = maSuKien, GhiChu = ghiChu }));

        public (bool ThanhCong, string ThongBao) HuyDangKy(string maDangKy, string maNguoiDung)
        {
            return KhoDuLieuAiven.Ghi(() =>
            {
                lock (_dongBo)
                {
                    if (!DichVuPhanQuyen.LaNguoiDungHienTai(maNguoiDung)) return (false, "Bạn không được hủy đăng ký của người khác.");
                    var don = _danhSachDangKy.FirstOrDefault(d => d.MaDangKy == maDangKy && d.MaNguoiDung == maNguoiDung);
                    if (don == null) return (false, "Không tìm thấy đăng ký.");
                    if (don.TrangThai != "ĐãĐăngKý") return (false, "Đăng ký đã hủy hoặc không thể hủy.");
                    var sk = LaySuKienTheoMa(don.MaSuKien);
                    if (sk == null || sk.TrangThai != "SắpDiễnRa" || sk.ThoiGianBatDau <= DateTime.Now)
                        return (false, "Chỉ được hủy đăng ký trước khi sự kiện bắt đầu.");
                    don.TrangThai = "ĐãHủy";
                    sk.SoLuongDaDangKy = Math.Max(0, sk.SoLuongDaDangKy - 1);
                    return (true, "Đã hủy đăng ký.");
                }
            });
        }

        public List<DangKySuKien> LayDangKyCuaNguoiDung(string maNguoiDung) => KhoDuLieuAiven.Doc(() => DichVuPhanQuyen.LaNguoiDungHienTai(maNguoiDung)
                ? _danhSachDangKy.Where(d => d.MaNguoiDung == maNguoiDung && d.TrangThai != "ĐãHủy").ToList()
                : new List<DangKySuKien>());

        public List<DangKySuKien> LayDangKyTheoNguoiDung(string maNguoiDung) => KhoDuLieuAiven.Doc(() => LayDangKyCuaNguoiDung(maNguoiDung));

        public List<DangKySuKien> LayDangKyTheoSuKien(string maSuKien)
        {
            return KhoDuLieuAiven.Doc(() =>
            {
                var sk = LaySuKienTheoMa(maSuKien);
                return sk != null && DichVuPhanQuyen.QuanLyCLB(new DichVuCLB().TimCLBTheoMa(sk.MaCLB))
                    ? _danhSachDangKy.Where(d => d.MaSuKien == maSuKien && d.TrangThai == "ĐãĐăngKý").ToList()
                    : new List<DangKySuKien>();
            });
        }

        private string KiemTraSuKien(SuKien sk)
        {
            if (sk == null || string.IsNullOrWhiteSpace(sk.TenSuKien) || string.IsNullOrWhiteSpace(sk.DiaDiem))
                return "Vui lòng nhập tên sự kiện và địa điểm.";
            var clb = new DichVuCLB().TimCLBTheoMa(sk.MaCLB);
            if (clb == null || !clb.TrangThaiHoatDong) return "Vui lòng chọn CLB đang hoạt động.";
            if (!DichVuPhanQuyen.QuanLyCLB(clb)) return "Bạn không được quản lý sự kiện của CLB này.";
            if (sk.ThoiGianBatDau <= DateTime.Now) return "Thời gian bắt đầu phải ở tương lai.";
            if (sk.ThoiGianKetThuc <= sk.ThoiGianBatDau) return "Thời gian kết thúc phải sau thời gian bắt đầu.";
            if (sk.SoLuongToiDa <= 0 || sk.SoLuongToiDa < sk.SoLuongDaDangKy)
                return "Số chỗ tối đa phải lớn hơn 0 và không ít hơn số người đã đăng ký.";
            if (!new[] { "HộiThảo", "VănHóaNghệThuật", "ThểThao", "HọcThuật" }.Contains(sk.LoaiSuKien))
                return "Vui lòng chọn loại sự kiện.";
            return null;
        }

        public (bool ThanhCong, string ThongBao) ThemSuKien(SuKien sk)
        {
            return KhoDuLieuAiven.Ghi(() =>
            {
                var loi = KiemTraSuKien(sk);
                if (loi != null) return (false, loi);
                sk.MaSuKien = Guid.NewGuid().ToString();
                sk.TenSuKien = sk.TenSuKien.Trim();
                sk.TenCLB = new DichVuCLB().TimCLBTheoMa(sk.MaCLB).TenCLB;
                sk.SoLuongDaDangKy = 0;
                sk.TrangThai = "SắpDiễnRa";
                _danhSachSuKien.Add(sk);
                return (true, "Đã tạo sự kiện.");
            });
        }

        public (bool ThanhCong, string ThongBao) CapNhatSuKien(SuKien capNhat)
        {
            return KhoDuLieuAiven.Ghi(() =>
            {
                var cu = LaySuKienTheoMa(capNhat?.MaSuKien);
                if (cu == null) return (false, "Không tìm thấy sự kiện.");
                if (!DichVuPhanQuyen.QuanLyCLB(new DichVuCLB().TimCLBTheoMa(cu.MaCLB)))
                    return (false, "Bạn không được sửa sự kiện này.");
                if (cu.TrangThai != "SắpDiễnRa") return (false, "Chỉ được sửa sự kiện chưa bắt đầu.");
                if (cu.MaCLB != capNhat.MaCLB && cu.SoLuongDaDangKy > 0)
                    return (false, "Không được đổi CLB khi sự kiện đã có người đăng ký.");
                capNhat.SoLuongDaDangKy = cu.SoLuongDaDangKy;
                var loi = KiemTraSuKien(capNhat);
                if (loi != null) return (false, loi);
                cu.TenSuKien = capNhat.TenSuKien.Trim();
                cu.MoTa = capNhat.MoTa;
                cu.MaCLB = capNhat.MaCLB;
                cu.TenCLB = new DichVuCLB().TimCLBTheoMa(cu.MaCLB).TenCLB;
                cu.DiaDiem = capNhat.DiaDiem;
                cu.ThoiGianBatDau = capNhat.ThoiGianBatDau;
                cu.ThoiGianKetThuc = capNhat.ThoiGianKetThuc;
                cu.SoLuongToiDa = capNhat.SoLuongToiDa;
                cu.LoaiSuKien = capNhat.LoaiSuKien;
                return (true, "Đã cập nhật sự kiện.");
            });
        }

        public (bool ThanhCong, string ThongBao) HuySuKien(string maSuKien)
        {
            return KhoDuLieuAiven.Ghi(() =>
            {
                lock (_dongBo)
                {
                    var sk = LaySuKienTheoMa(maSuKien);
                    if (sk == null) return (false, "Không tìm thấy sự kiện.");
                    if (!DichVuPhanQuyen.QuanLyCLB(new DichVuCLB().TimCLBTheoMa(sk.MaCLB)))
                        return (false, "Bạn không được hủy sự kiện này.");
                    if (sk.TrangThai != "SắpDiễnRa") return (false, "Chỉ được hủy sự kiện chưa bắt đầu.");
                    sk.TrangThai = "ĐãHủy";
                    foreach (var don in _danhSachDangKy.Where(d => d.MaSuKien == maSuKien && d.TrangThai == "ĐãĐăngKý"))
                        don.TrangThai = "SựKiệnĐãHủy";
                    sk.SoLuongDaDangKy = 0;
                    return (true, "Đã hủy sự kiện và cập nhật các đăng ký liên quan.");
                }
            });
        }

        public List<SuKien> LaySuKienTheoThang(int thang, int nam) => KhoDuLieuAiven.Doc(() => LayDanhSachSuKien().Where(s => s.TrangThai != "ĐãHủy" &&
                s.ThoiGianBatDau < new DateTime(nam, thang, 1).AddMonths(1) &&
                s.ThoiGianKetThuc > new DateTime(nam, thang, 1)).OrderBy(s => s.ThoiGianBatDau).ToList());

        public SuKien LaySuKienTheoMa(string maSuKien)
        {
            return KhoDuLieuAiven.Doc(() =>
            {
                CapNhatTrangThai();
                return _danhSachSuKien.FirstOrDefault(s => s.MaSuKien == maSuKien);
            });
        }
    }
}
