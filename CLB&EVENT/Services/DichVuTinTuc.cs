using System;
using System.Collections.Generic;
using System.Linq;
using CLB_EVENT.Models;
using CLB_EVENT.DuLieu;

namespace CLB_EVENT.Services
{
    /// <summary>
    /// Dịch vụ quản lý Tin Tức và Thông Báo trong hệ thống
    /// </summary>
    public class DichVuTinTuc
    {
        // Snapshot tin tức được đồng bộ từ MySQL.
        private static List<TinTuc> _danhSachTinTuc = new List<TinTuc>();
        internal static List<TinTuc> DuLieuTinTuc => _danhSachTinTuc;

        /// <summary>
        /// Khởi tạo dữ liệu mẫu khi tải lên
        /// </summary>
        static DichVuTinTuc()
        {
            KhoiTaoDuLieuMau();
        }

        /// <summary>
        /// Tạo dữ liệu tin tức mẫu ban đầu
        /// </summary>
        private static void KhoiTaoDuLieuMau()
        {
            _danhSachTinTuc.AddRange(new[]
            {
                new TinTuc
                {
                    MaTinTuc = "TT001",
                    TieuDe = "Thông báo tuyển thành viên CLB Công Nghệ Thông Tin năm học 2026-2027",
                    TomTat = "CLB CNTT mở đơn tuyển thành viên mới cho năm học 2026-2027. Sinh viên quan tâm vui lòng nộp đơn trước ngày 15/10/2026.",
                    NoiDung = "CLB Công Nghệ Thông Tin trân trọng thông báo tuyển dụng thành viên mới cho năm học 2026-2027. " +
                              "Đây là cơ hội tuyệt vời để các bạn sinh viên yêu thích công nghệ tham gia vào một cộng đồng năng động. " +
                              "CLB cung cấp môi trường học tập, thực hành lập trình, phát triển dự án và kết nối với doanh nghiệp.",
                    TacGia = "Ban Chủ Nhiệm CLB CNTT",
                    NgayDang = DateTime.Now.AddDays(-2),
                    DanhMuc = "ThôngBáo",
                    MaCLB = "CLB001",
                    LuotXem = 245
                },
                new TinTuc
                {
                    MaTinTuc = "TT002",
                    TieuDe = "Hackathon 2026 - Cuộc thi lập trình lớn nhất năm",
                    TomTat = "CLB CNTT tổ chức Hackathon 24 giờ với tổng giải thưởng lên đến 50 triệu đồng. Đăng ký ngay!",
                    NoiDung = "Hackathon 2026 là sự kiện lập trình hấp dẫn nhất trong năm do CLB CNTT tổ chức. " +
                              "Với tổng giải thưởng lên đến 50 triệu đồng, đây là cơ hội để sinh viên thể hiện tài năng. " +
                              "Chủ đề năm nay: 'Công nghệ xanh vì tương lai bền vững'.",
                    TacGia = "BTC Hackathon 2026",
                    NgayDang = DateTime.Now.AddDays(-5),
                    DanhMuc = "SựKiện",
                    MaCLB = "CLB001",
                    LuotXem = 512
                },
                new TinTuc
                {
                    MaTinTuc = "TT003",
                    TieuDe = "Đêm Văn Nghệ Mùa Thu - Chương trình không thể bỏ lỡ",
                    TomTat = "CLB Văn Hóa Nghệ Thuật chuẩn bị Đêm Văn Nghệ Mùa Thu hoành tráng với hơn 20 tiết mục đặc sắc.",
                    NoiDung = "Đêm Văn Nghệ Mùa Thu 2026 hứa hẹn là một đêm đầy màu sắc với hơn 20 tiết mục nghệ thuật. " +
                              "Từ ca nhạc, múa, kịch đến các tiết mục đặc biệt bất ngờ. Sự kiện hoàn toàn miễn phí cho tất cả sinh viên.",
                    TacGia = "CLB Văn Hóa Nghệ Thuật",
                    NgayDang = DateTime.Now.AddDays(-8),
                    DanhMuc = "HoạtĐộngSinhViên",
                    MaCLB = "CLB003",
                    LuotXem = 388
                },
                new TinTuc
                {
                    MaTinTuc = "TT004",
                    TieuDe = "Giải Bóng Đá Sinh Viên 2026 - Tranh Cúp Vô Địch",
                    TomTat = "CLB Thể Thao tổ chức giải bóng đá 7 người dành cho các khoa, tranh cúp vô địch trường.",
                    NoiDung = "Giải Bóng Đá Sinh Viên 2026 sẽ diễn ra trong 5 ngày với sự tham gia của tất cả các khoa. " +
                              "Mỗi khoa đăng ký một đội gồm 10 cầu thủ. Vô địch nhận cúp và phần thưởng tiền mặt 5 triệu đồng.",
                    TacGia = "CLB Thể Thao",
                    NgayDang = DateTime.Now.AddDays(-10),
                    DanhMuc = "SựKiện",
                    MaCLB = "CLB004",
                    LuotXem = 302
                },
                new TinTuc
                {
                    MaTinTuc = "TT005",
                    TieuDe = "Workshop Tiếng Anh Chuyên Ngành - Hết chỗ trước 2 ngày!",
                    TomTat = "Workshop Kỹ Năng Tiếng Anh Chuyên Ngành đã đăng ký hết 50 suất. Mở thêm đợt 2 vào tháng 11.",
                    NoiDung = "Workshop Kỹ Năng Thuyết Trình Tiếng Anh do CLB Tiếng Anh tổ chức đã nhận được phản hồi tích cực. " +
                              "50 suất đăng ký đã được lấp đầy chỉ sau 2 ngày mở đăng ký. " +
                              "CLB sẽ mở thêm đợt 2 vào tháng 11, sinh viên quan tâm hãy theo dõi thông báo mới nhất.",
                    TacGia = "CLB Tiếng Anh",
                    NgayDang = DateTime.Now.AddDays(-12),
                    DanhMuc = "ThôngBáo",
                    MaCLB = "CLB005",
                    LuotXem = 178
                },
                new TinTuc
                {
                    MaTinTuc = "TT006",
                    TieuDe = "Ngày Hội CLB - Giới thiệu các CLB trường đến tân sinh viên K21",
                    TomTat = "Ngày Hội CLB sẽ diễn ra vào cuối tháng 10, là cơ hội để tân sinh viên tìm hiểu và đăng ký các CLB.",
                    NoiDung = "Ngày Hội CLB Trường Đại Học là sự kiện thường niên, là cơ hội để các CLB quảng bá hoạt động " +
                              "và tuyển thành viên mới, đặc biệt là tân sinh viên K21. " +
                              "Hơn 15 CLB sẽ có gian hàng giới thiệu với nhiều hoạt động thú vị.",
                    TacGia = "Phòng Công Tác Sinh Viên",
                    NgayDang = DateTime.Now.AddDays(-15),
                    DanhMuc = "HoạtĐộngSinhViên",
                    MaCLB = null,
                    LuotXem = 621
                }
            });
        }

        /// <summary>
        /// Lấy danh sách tất cả tin tức đã duyệt
        /// </summary>
        public List<TinTuc> LayDanhSachTinTuc() => KhoDuLieuAiven.Doc(() => _danhSachTinTuc.FindAll(t => t.TrangThai == "ĐãDuyệt"));

        /// <summary>
        /// Tìm kiếm tin tức theo từ khóa
        /// </summary>
        public List<TinTuc> TimKiemTinTuc(string tuKhoa) => KhoDuLieuAiven.Doc(() => string.IsNullOrWhiteSpace(tuKhoa)
                ? LayDanhSachTinTuc()
                : _danhSachTinTuc.FindAll(t =>
                    t.TrangThai == "ĐãDuyệt" &&
                    ((t.TieuDe != null && t.TieuDe.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0) ||
                      (t.TomTat != null && t.TomTat.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0) ||
                      (t.NoiDung != null && t.NoiDung.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0))));

        /// <summary>
        /// Lọc tin tức theo danh mục
        /// </summary>
        public List<TinTuc> LayTinTucTheoDanhMuc(string danhMuc) => KhoDuLieuAiven.Doc(() => _danhSachTinTuc.FindAll(t => t.TrangThai == "ĐãDuyệt" && t.DanhMuc == danhMuc));

        public TinTuc DocTin(string maTin)
        {
            return KhoDuLieuAiven.Ghi(() =>
            {
                var tin = _danhSachTinTuc.FirstOrDefault(t => t.MaTinTuc == maTin && t.TrangThai == "ĐãDuyệt");
                if (tin != null) tin.LuotXem++;
                return tin;
            });
        }

        public List<TinTuc> LayTinDuocQuanLy() => KhoDuLieuAiven.Doc(() => _danhSachTinTuc.Where(CoQuyenQuanLy).ToList());

        private bool CoQuyenQuanLy(TinTuc tin) => tin != null && (DichVuPhanQuyen.LaAdmin ||
            (!string.IsNullOrWhiteSpace(tin.MaCLB) && DichVuPhanQuyen.QuanLyCLB(new DichVuCLB().TimCLBTheoMa(tin.MaCLB))));

        public (bool ThanhCong, string ThongBao) LuuTin(TinTuc tin, bool themMoi)
        {
            return KhoDuLieuAiven.Ghi(() =>
            {
                if (tin == null || !CoQuyenQuanLy(tin)) return (false, "Bạn không có quyền đăng tin cho đơn vị này.");
                if (!string.IsNullOrEmpty(tin.MaCLB) && new DichVuCLB().TimCLBTheoMa(tin.MaCLB)?.TrangThaiHoatDong != true)
                    return (false, "CLB không còn hoạt động.");
                if (string.IsNullOrWhiteSpace(tin.TieuDe) || string.IsNullOrWhiteSpace(tin.NoiDung))
                    return (false, "Vui lòng nhập tiêu đề và nội dung.");
                if (!new[] { "ThôngBáo", "SựKiện", "HoạtĐộngSinhViên", "CLB" }.Contains(tin.DanhMuc))
                    return (false, "Vui lòng chọn danh mục hợp lệ.");
                var cu = _danhSachTinTuc.FirstOrDefault(t => t.MaTinTuc == tin.MaTinTuc);
                if (!themMoi && (cu == null || !CoQuyenQuanLy(cu))) return (false, "Bạn không được sửa bài tin này.");
                if (themMoi)
                {
                    cu = new TinTuc();
                    _danhSachTinTuc.Add(cu);
                }
                cu.TieuDe = tin.TieuDe.Trim();
                cu.NoiDung = tin.NoiDung.Trim();
                cu.TomTat = string.IsNullOrWhiteSpace(tin.TomTat) ? cu.NoiDung.Substring(0, Math.Min(160, cu.NoiDung.Length)) : tin.TomTat.Trim();
                cu.DanhMuc = tin.DanhMuc;
                cu.MaCLB = tin.MaCLB;
                cu.TacGia = DichVuNguoiDung.NguoiDungHienTai.HoTen;
                cu.TrangThai = "ĐãDuyệt";
                return (true, "Đã lưu bài tin.");
            });
        }

        public (bool ThanhCong, string ThongBao) AnTin(string maTin)
        {
            return KhoDuLieuAiven.Ghi(() =>
            {
                var tin = _danhSachTinTuc.FirstOrDefault(t => t.MaTinTuc == maTin);
                if (!CoQuyenQuanLy(tin)) return (false, "Bạn không được ẩn bài tin này.");
                tin.TrangThai = "ĐãẨn";
                return (true, "Đã ẩn bài tin.");
            });
        }
    }
}
