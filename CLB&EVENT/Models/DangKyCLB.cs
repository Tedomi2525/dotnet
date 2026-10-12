using System;

namespace CLB_EVENT.Models
{
    /// <summary>
    /// Lớp đại diện cho bản đăng ký tham gia CLB của sinh viên
    /// </summary>
    public class DangKyCLB
    {
        /// <summary>
        /// Mã định danh duy nhất của bản đăng ký
        /// </summary>
        public string MaDangKy { get; set; }

        /// <summary>
        /// Mã người dùng đăng ký
        /// </summary>
        public string MaNguoiDung { get; set; }

        /// <summary>
        /// Họ tên người đăng ký
        /// </summary>
        public string HoTen { get; set; }

        /// <summary>
        /// Tên người dùng (alias của HoTen)
        /// </summary>
        public string TenNguoiDung
        {
            get { return HoTen; }
            set { HoTen = value; }
        }

        /// <summary>
        /// Mã sinh viên
        /// </summary>
        public string MaSinhVien { get; set; }

        /// <summary>
        /// Lớp học
        /// </summary>
        public string LopHoc { get; set; }

        /// <summary>
        /// Email liên hệ
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Số điện thoại liên hệ
        /// </summary>
        public string SoDienThoai { get; set; }

        /// <summary>
        /// Mã CLB muốn tham gia
        /// </summary>
        public string MaCLB { get; set; }

        /// <summary>
        /// Tên CLB muốn tham gia
        /// </summary>
        public string TenCLB { get; set; }

        /// <summary>
        /// Lý do muốn tham gia CLB
        /// </summary>
        public string LyDo { get; set; }

        /// <summary>
        /// Kỹ năng hoặc kinh nghiệm liên quan
        /// </summary>
        public string KyNang { get; set; }

        /// <summary>
        /// Thời gian có thể tham gia hoạt động CLB
        /// </summary>
        public string ThoiGianRanh { get; set; }

        /// <summary>
        /// Thời gian đăng ký
        /// </summary>
        public DateTime ThoiGianDangKy { get; set; }

        /// <summary>
        /// Ngày đăng ký (alias của ThoiGianDangKy)
        /// </summary>
        public DateTime NgayDangKy
        {
            get { return ThoiGianDangKy; }
            set { ThoiGianDangKy = value; }
        }

        /// <summary>
        /// Trạng thái đăng ký (ChờDuyệt, ĐãDuyệt, TừChối)
        /// </summary>
        public string TrangThai { get; set; }

        public DangKyCLB()
        {
            MaDangKy = Guid.NewGuid().ToString();
            ThoiGianDangKy = DateTime.Now;
            TrangThai = "ChờDuyệt";
        }
    }
}
