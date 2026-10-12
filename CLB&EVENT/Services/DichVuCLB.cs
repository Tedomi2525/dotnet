using System;
using System.Collections.Generic;
using System.Linq;
using CLB_EVENT.Models;
using CLB_EVENT.DuLieu;

namespace CLB_EVENT.Services
{
    /// <summary>
    /// Dịch vụ quản lý Câu Lạc Bộ: thêm, sửa, xóa, tra cứu CLB
    /// và xử lý đơn đăng ký tham gia CLB
    /// </summary>
    public class DichVuCLB
    {
        // Snapshot CLB được đồng bộ từ MySQL.
        private static List<CauLacBo> _danhSachCLB = new List<CauLacBo>();
        internal static List<CauLacBo> DuLieuCLB => _danhSachCLB;
        internal static List<DangKyCLB> DuLieuDangKy => _danhSachDangKy;

        // Danh sách đơn đăng ký tham gia CLB
        private static List<DangKyCLB> _danhSachDangKy = new List<DangKyCLB>();

        /// <summary>
        /// Khởi tạo dữ liệu mẫu cho các CLB
        /// </summary>
        static DichVuCLB()
        {
            KhoiTaoDuLieuMau();
        }

        /// <summary>
        /// Tạo dữ liệu CLB mẫu ban đầu
        /// </summary>
        private static void KhoiTaoDuLieuMau()
        {
            _danhSachCLB.AddRange(new[]
            {
                new CauLacBo
                {
                    MaCLB = "CLB001",
                    MaNguoiDungQuanLy = "ND003",
                    TenCLB = "CLB Công Nghệ Thông Tin",
                    MoTa = "CLB dành cho sinh viên yêu thích lập trình, phát triển phần mềm và công nghệ. " +
                           "Tổ chức các buổi workshop, hackathon và cuộc thi lập trình.",
                    Email = "clbcntt@truong.edu.vn",
                    SoDienThoai = "0901111111",
                    ChuTichCLB = "Nguyễn Văn An",
                    SoThanhVien = 85,
                    PhongBan = "Khoa Công Nghệ Thông Tin",
                    NgayThanhLap = new DateTime(2018, 9, 1),
                    DiaDiemHoatDong = "Phòng lab máy tính - Tòa A",
                    TrangThaiHoatDong = true
                },
                new CauLacBo
                {
                    MaCLB = "CLB002",
                    TenCLB = "CLB Nghiên Cứu Khoa Học",
                    MoTa = "CLB khuyến khích nghiên cứu khoa học, đổi mới sáng tạo trong sinh viên. " +
                           "Hỗ trợ sinh viên tham gia các đề tài nghiên cứu cấp trường và quốc gia.",
                    Email = "clbnckh@truong.edu.vn",
                    SoDienThoai = "0902222222",
                    ChuTichCLB = "Trần Thị Minh",
                    SoThanhVien = 45,
                    PhongBan = "Phòng Khoa Học & Công Nghệ",
                    NgayThanhLap = new DateTime(2019, 3, 15),
                    DiaDiemHoatDong = "Phòng nghiên cứu - Tòa B",
                    TrangThaiHoatDong = true
                },
                new CauLacBo
                {
                    MaCLB = "CLB003",
                    TenCLB = "CLB Văn Hóa Nghệ Thuật",
                    MoTa = "CLB thúc đẩy năng khiếu nghệ thuật: ca hát, múa, kịch và các loại hình nghệ thuật khác. " +
                           "Đại diện trường trong các hội diễn văn nghệ.",
                    Email = "clbvhnt@truong.edu.vn",
                    SoDienThoai = "0903333333",
                    ChuTichCLB = "Lê Thị Thu Hương",
                    SoThanhVien = 120,
                    PhongBan = "Phòng Công Tác Sinh Viên",
                    NgayThanhLap = new DateTime(2017, 10, 20),
                    DiaDiemHoatDong = "Hội trường lớn - Tòa C",
                    TrangThaiHoatDong = true
                },
                new CauLacBo
                {
                    MaCLB = "CLB004",
                    TenCLB = "CLB Thể Thao",
                    MoTa = "CLB phát triển thể chất: bóng đá, bóng rổ, cầu lông, bơi lội. " +
                           "Tổ chức giải thể thao nội trường và giao lưu với các trường bạn.",
                    Email = "clbthethao@truong.edu.vn",
                    SoDienThoai = "0904444444",
                    ChuTichCLB = "Phạm Quang Huy",
                    SoThanhVien = 200,
                    PhongBan = "Phòng Công Tác Sinh Viên",
                    NgayThanhLap = new DateTime(2016, 8, 1),
                    DiaDiemHoatDong = "Sân thể thao - Khu D",
                    TrangThaiHoatDong = true
                },
                new CauLacBo
                {
                    MaCLB = "CLB005",
                    TenCLB = "CLB Tiếng Anh",
                    MoTa = "CLB nâng cao kỹ năng tiếng Anh: giao tiếp, thuyết trình, IELTS. " +
                           "Tổ chức English Corner, cuộc thi hùng biện tiếng Anh.",
                    Email = "clbtienganh@truong.edu.vn",
                    SoDienThoai = "0905555555",
                    ChuTichCLB = "Hoàng Thị Lan",
                    SoThanhVien = 95,
                    PhongBan = "Khoa Ngoại Ngữ",
                    NgayThanhLap = new DateTime(2018, 1, 10),
                    DiaDiemHoatDong = "Phòng học Ngoại Ngữ - Tòa A",
                    TrangThaiHoatDong = true
                },
                new CauLacBo
                {
                    MaCLB = "CLB006",
                    TenCLB = "CLB Khởi Nghiệp",
                    MoTa = "CLB hỗ trợ sinh viên khởi nghiệp, xây dựng ý tưởng kinh doanh, " +
                           "kết nối với doanh nghiệp và các quỹ đầu tư.",
                    Email = "clbkhoinghiep@truong.edu.vn",
                    SoDienThoai = "0906666666",
                    ChuTichCLB = "Vũ Đình Tùng",
                    SoThanhVien = 60,
                    PhongBan = "Khoa Kinh Tế",
                    NgayThanhLap = new DateTime(2020, 5, 1),
                    DiaDiemHoatDong = "Phòng Innovation Hub - Tòa B",
                    TrangThaiHoatDong = true
                }
            });
        }

        public List<CauLacBo> LayDanhSachCLB() => KhoDuLieuAiven.Doc(() => _danhSachCLB.Where(c => c.TrangThaiHoatDong).ToList());

        public List<CauLacBo> LayTatCaCLB() => KhoDuLieuAiven.Doc(() => new List<CauLacBo>(_danhSachCLB));

        public List<CauLacBo> LayCLBDuocQuanLy() => KhoDuLieuAiven.Doc(() => LayDanhSachCLB().Where(DichVuPhanQuyen.QuanLyCLB).ToList());

        public CauLacBo TimCLBTheoMa(string maCLB) => KhoDuLieuAiven.Doc(() => _danhSachCLB.FirstOrDefault(c => c.MaCLB == maCLB));

        public CauLacBo LayCLBTheoTen(string tenCLB) => KhoDuLieuAiven.Doc(() => _danhSachCLB.FirstOrDefault(c => c.TrangThaiHoatDong &&
                string.Equals(c.TenCLB, tenCLB, StringComparison.OrdinalIgnoreCase)));

        public List<CauLacBo> TimKiemCLB(string tuKhoa) => KhoDuLieuAiven.Doc(() => LayDanhSachCLB().Where(c => string.IsNullOrWhiteSpace(tuKhoa) ||
                (c.TenCLB ?? "").IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0 ||
                (c.MoTa ?? "").IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0 ||
                (c.PhongBan ?? "").IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0).ToList());

        private string KiemTraCLB(CauLacBo clb)
        {
            if (clb == null || string.IsNullOrWhiteSpace(clb.TenCLB))
                return "Vui lòng nhập tên CLB.";
            if (_danhSachCLB.Any(c => c.MaCLB != clb.MaCLB &&
                string.Equals(c.TenCLB, clb.TenCLB.Trim(), StringComparison.OrdinalIgnoreCase)))
                return "Tên CLB đã tồn tại.";
            if (!string.IsNullOrWhiteSpace(clb.Email) && !DichVuNguoiDung.EmailHopLe(clb.Email))
                return "Email CLB không đúng định dạng.";
            if (!DichVuNguoiDung.SoDienThoaiHopLe(clb.SoDienThoai))
                return "Số điện thoại CLB không đúng định dạng.";
            if (!string.IsNullOrWhiteSpace(clb.MaNguoiDungQuanLy))
            {
                var manager = new DichVuNguoiDung().TimNguoiDungTheoMa(clb.MaNguoiDungQuanLy);
                if (manager == null || !manager.TrangThaiHoatDong || manager.VaiTro != "QuanLyCLB")
                    return "Vui lòng chọn tài khoản quản lý CLB đang hoạt động.";
            }
            return null;
        }

        public (bool ThanhCong, string ThongBao) ThemCLBCoThongBao(CauLacBo clbMoi)
        {
            return KhoDuLieuAiven.Ghi(() =>
            {
                if (!DichVuPhanQuyen.LaAdmin) return (false, "Chỉ quản trị viên được thêm CLB.");
                var loi = KiemTraCLB(clbMoi);
                if (loi != null) return (false, loi);
                clbMoi.MaCLB = Guid.NewGuid().ToString();
                clbMoi.TenCLB = clbMoi.TenCLB.Trim();
                clbMoi.TrangThaiHoatDong = true;
                clbMoi.SoThanhVien = 0;
                _danhSachCLB.Add(clbMoi);
                return (true, "Thêm CLB thành công.");
            });
        }

        public bool ThemCLB(CauLacBo clbMoi) => KhoDuLieuAiven.Ghi(() => ThemCLBCoThongBao(clbMoi).ThanhCong);

        public (bool ThanhCong, string ThongBao) CapNhatCLBCoThongBao(CauLacBo clbCapNhat)
        {
            return KhoDuLieuAiven.Ghi(() =>
            {
                var clbCu = TimCLBTheoMa(clbCapNhat?.MaCLB);
                if (clbCu == null || !clbCu.TrangThaiHoatDong) return (false, "Không tìm thấy CLB đang hoạt động.");
                if (!DichVuPhanQuyen.QuanLyCLB(clbCu)) return (false, "Bạn không có quyền sửa CLB này.");
                if (!DichVuPhanQuyen.LaAdmin && clbCapNhat.MaNguoiDungQuanLy != clbCu.MaNguoiDungQuanLy)
                    return (false, "Chỉ quản trị viên được phân công quản lý CLB.");
                var loi = KiemTraCLB(clbCapNhat);
                if (loi != null) return (false, loi);
                clbCu.TenCLB = clbCapNhat.TenCLB.Trim();
                clbCu.MoTa = clbCapNhat.MoTa;
                clbCu.Email = clbCapNhat.Email;
                clbCu.SoDienThoai = clbCapNhat.SoDienThoai;
                clbCu.ChuTichCLB = clbCapNhat.ChuTichCLB;
                clbCu.PhongBan = clbCapNhat.PhongBan;
                clbCu.DiaDiemHoatDong = clbCapNhat.DiaDiemHoatDong;
                clbCu.MaNguoiDungQuanLy = clbCapNhat.MaNguoiDungQuanLy;
                return (true, "Cập nhật CLB thành công.");
            });
        }

        public bool CapNhatCLB(CauLacBo clbCapNhat) => KhoDuLieuAiven.Ghi(() => CapNhatCLBCoThongBao(clbCapNhat).ThanhCong);

        public (bool ThanhCong, string ThongBao) XoaCLBCoThongBao(string maCLB)
        {
            return KhoDuLieuAiven.Ghi(() =>
            {
                if (!DichVuPhanQuyen.LaAdmin) return (false, "Chỉ quản trị viên được xóa CLB.");
                var clb = TimCLBTheoMa(maCLB);
                if (clb == null || !clb.TrangThaiHoatDong) return (false, "CLB không tồn tại hoặc đã ngừng hoạt động.");
                if (_danhSachDangKy.Any(d => d.MaCLB == maCLB && d.TrangThai == "ChờDuyệt"))
                    return (false, "CLB còn đơn đăng ký chờ duyệt. Hãy xử lý các đơn trước.");
                if (new DichVuSuKien().LayDanhSachSuKien().Any(s => s.MaCLB == maCLB &&
                    s.TrangThai != "ĐãHủy" && s.ThoiGianKetThuc > DateTime.Now))
                    return (false, "CLB còn sự kiện chưa kết thúc. Hãy hủy hoặc hoàn tất các sự kiện trước.");
                clb.TrangThaiHoatDong = false;
                return (true, "Đã ngừng hoạt động CLB. Lịch sử được giữ lại.");
            });
        }

        public bool XoaCLB(string maCLB) => KhoDuLieuAiven.Ghi(() => XoaCLBCoThongBao(maCLB).ThanhCong);

        public (bool ThanhCong, string ThongBao) DangKyVaoCLB(DangKyCLB don)
        {
            return KhoDuLieuAiven.Ghi(() =>
            {
                if (don == null || !DichVuPhanQuyen.LaNguoiDungHienTai(don.MaNguoiDung))
                    return (false, "Vui lòng đăng nhập bằng tài khoản nộp đơn.");
                var clb = TimCLBTheoMa(don.MaCLB);
                if (clb == null || !clb.TrangThaiHoatDong) return (false, "CLB không còn hoạt động.");
                if (string.IsNullOrWhiteSpace(don.LyDo)) return (false, "Vui lòng nhập lý do tham gia.");
                if (_danhSachDangKy.Any(d => d.MaNguoiDung == don.MaNguoiDung &&
                    d.MaCLB == don.MaCLB && d.TrangThai != "TừChối"))
                    return (false, "Bạn đã là thành viên hoặc đang có đơn chờ duyệt.");
                var user = DichVuNguoiDung.NguoiDungHienTai;
                don.MaDangKy = Guid.NewGuid().ToString();
                don.HoTen = user.HoTen;
                don.Email = user.Email;
                don.SoDienThoai = user.SoDienThoai;
                don.LopHoc = user.LopHoc;
                don.TenCLB = clb.TenCLB;
                don.LyDo = don.LyDo.Trim();
                don.TrangThai = "ChờDuyệt";
                don.ThoiGianDangKy = DateTime.Now;
                _danhSachDangKy.Add(don);
                return (true, "Gửi đơn thành công. Vui lòng chờ quản lý CLB xét duyệt.");
            });
        }

        public (bool ThanhCong, string ThongBao) NopDonDangKyCLB(DangKyCLB don) => KhoDuLieuAiven.Ghi(() => DangKyVaoCLB(don));

        public List<DangKyCLB> LayDanhSachDangKyTheoCLB(string maCLB) => KhoDuLieuAiven.Doc(() => DichVuPhanQuyen.QuanLyCLB(TimCLBTheoMa(maCLB))
                ? _danhSachDangKy.Where(d => d.MaCLB == maCLB).ToList()
                : new List<DangKyCLB>());

        public List<DangKyCLB> LayDangKyCuaNguoiDung(string maNguoiDung) => KhoDuLieuAiven.Doc(() => DichVuPhanQuyen.LaNguoiDungHienTai(maNguoiDung)
                ? _danhSachDangKy.Where(d => d.MaNguoiDung == maNguoiDung).ToList()
                : new List<DangKyCLB>());

        public List<DangKyCLB> LayDangKyTheoNguoiDung(string maNguoiDung) => KhoDuLieuAiven.Doc(() => LayDangKyCuaNguoiDung(maNguoiDung));

        public List<DangKyCLB> LayDonDangKyChoDuyet() => KhoDuLieuAiven.Doc(() => _danhSachDangKy.Where(d => d.TrangThai == "ChờDuyệt" &&
                DichVuPhanQuyen.QuanLyCLB(TimCLBTheoMa(d.MaCLB))).ToList());

        public (bool ThanhCong, string ThongBao) XuLyDonDangKy(string maDangKy, bool chapNhan)
        {
            return KhoDuLieuAiven.Ghi(() =>
            {
                var don = _danhSachDangKy.FirstOrDefault(d => d.MaDangKy == maDangKy);
                if (don == null) return (false, "Không tìm thấy đơn đăng ký.");
                var clb = TimCLBTheoMa(don.MaCLB);
                if (!DichVuPhanQuyen.QuanLyCLB(clb)) return (false, "Bạn không được duyệt đơn của CLB này.");
                if (!clb.TrangThaiHoatDong) return (false, "CLB không còn hoạt động.");
                if (don.TrangThai != "ChờDuyệt") return (false, "Đơn này đã được xử lý.");
                don.TrangThai = chapNhan ? "ĐãDuyệt" : "TừChối";
                if (chapNhan) clb.SoThanhVien++;
                return (true, chapNhan ? "Đã duyệt đơn và thêm thành viên." : "Đã từ chối đơn.");
            });
        }
    }
}
