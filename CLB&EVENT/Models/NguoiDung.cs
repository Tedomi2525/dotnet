using System;

namespace CLB_EVENT.Models
{
    /// <summary>
    /// Lớp đại diện cho người dùng trong hệ thống (Sinh viên, Quản trị viên, Quản lý CLB)
    /// </summary>
    public class NguoiDung
    {
        /// <summary>
        /// Mã định danh duy nhất của người dùng
        /// </summary>
        public string MaNguoiDung { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string HoTen { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string TenDangNhap { get; set; }

        /// <summary>
        /// Mật khẩu đã băm SHA-256
        /// </summary>
        public string MatKhauMaHoa { get; set; }

        /// <summary>
        /// Email liên hệ
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string SoDienThoai { get; set; }

        /// <summary>
        /// Vai trò: QuanLyAdmin, SinhVien, QuanLyCLB
        /// </summary>
        public string VaiTro { get; set; }

        /// <summary>
        /// Lớp học hoặc đơn vị công tác
        /// </summary>
        public string LopHoc { get; set; }

        /// <summary>
        /// Ngày tạo tài khoản
        /// </summary>
        public DateTime NgayTao { get; set; }

        /// <summary>
        /// Trạng thái hoạt động (true: đang hoạt động, false: bị khóa)
        /// </summary>
        public bool TrangThaiHoatDong { get; set; }

        public NguoiDung()
        {
            MaNguoiDung = Guid.NewGuid().ToString();
            NgayTao = DateTime.Now;
            TrangThaiHoatDong = true;
            VaiTro = "SinhVien";
        }
    }
}
