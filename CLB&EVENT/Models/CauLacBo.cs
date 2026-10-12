using System;

namespace CLB_EVENT.Models
{
    /// <summary>
    /// Lớp đại diện cho Câu Lạc Bộ trong hệ thống
    /// </summary>
    public class CauLacBo
    {
        /// <summary>
        /// Mã định danh của CLB (ví dụ: CLB001)
        /// </summary>
        public string MaCLB { get; set; }

        /// <summary>
        /// Tên đầy đủ của Câu Lạc Bộ
        /// </summary>
        public string TenCLB { get; set; }

        /// <summary>
        /// Giới thiệu, mô tả về CLB
        /// </summary>
        public string MoTa { get; set; }

        /// <summary>
        /// Email liên hệ của CLB
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Số điện thoại liên hệ
        /// </summary>
        public string SoDienThoai { get; set; }

        /// <summary>
        /// Họ tên Chủ tịch hoặc Trưởng ban CLB
        /// </summary>
        public string ChuTichCLB { get; set; }

        public string MaNguoiDungQuanLy { get; set; }

        /// <summary>
        /// Tên chủ nhiệm CLB (alias của ChuTichCLB, dùng cho giao diện mới)
        /// </summary>
        public string ChuNhiem
        {
            get { return ChuTichCLB; }
            set { ChuTichCLB = value; }
        }

        /// <summary>
        /// Thông tin liên hệ tổng hợp (email hoặc số điện thoại)
        /// </summary>
        public string ThongTinLienHe
        {
            get { return string.IsNullOrEmpty(Email) ? SoDienThoai : Email; }
            set { Email = value; }
        }

        /// <summary>
        /// Số lượng thành viên hiện tại
        /// </summary>
        public int SoThanhVien { get; set; }

        /// <summary>
        /// Đơn vị / Khoa trực thuộc phụ trách
        /// </summary>
        public string PhongBan { get; set; }

        /// <summary>
        /// Ngày thành lập CLB
        /// </summary>
        public DateTime NgayThanhLap { get; set; }

        /// <summary>
        /// Địa điểm sinh hoạt / hoạt động chính
        /// </summary>
        public string DiaDiemHoatDong { get; set; }

        /// <summary>
        /// Trạng thái hoạt động (true: đang hoạt động, false: tạm dừng/giải thể)
        /// </summary>
        public bool TrangThaiHoatDong { get; set; }

        public CauLacBo()
        {
            MaCLB = Guid.NewGuid().ToString();
            NgayThanhLap = DateTime.Now;
            TrangThaiHoatDong = true;
            SoThanhVien = 0;
        }
    }
}
