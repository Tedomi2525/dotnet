using System;

namespace CLB_EVENT.Models
{
    /// <summary>
    /// Lớp đại diện cho một bản đăng ký sự kiện của sinh viên
    /// </summary>
    public class DangKySuKien
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
        /// Mã sự kiện được đăng ký
        /// </summary>
        public string MaSuKien { get; set; }

        /// <summary>
        /// Tên sự kiện
        /// </summary>
        public string TenSuKien { get; set; }

        /// <summary>
        /// Thời gian đăng ký
        /// </summary>
        public DateTime ThoiGianDangKy { get; set; }

        /// <summary>
        /// Trạng thái đăng ký (ChờDuyệt, ĐãDuyệt, TừChối, ĐãHủy)
        /// </summary>
        public string TrangThai { get; set; }

        /// <summary>
        /// Ghi chú của người đăng ký
        /// </summary>
        public string GhiChu { get; set; }

        /// <summary>
        /// Email liên hệ khi đăng ký
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Số điện thoại liên hệ
        /// </summary>
        public string SoDienThoai { get; set; }

        /// <summary>
        /// Ngày đăng ký (alias của ThoiGianDangKy)
        /// </summary>
        public DateTime NgayDangKy
        {
            get { return ThoiGianDangKy; }
            set { ThoiGianDangKy = value; }
        }

        public DangKySuKien()
        {
            MaDangKy = Guid.NewGuid().ToString();
            ThoiGianDangKy = DateTime.Now;
            TrangThai = "ĐãĐăngKý";
        }
    }
}
