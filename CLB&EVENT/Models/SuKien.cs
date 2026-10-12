using System;

namespace CLB_EVENT.Models
{
    /// <summary>
    /// Lớp đại diện cho một Sự kiện trong hệ thống
    /// </summary>
    public class SuKien
    {
        /// <summary>
        /// Mã định danh duy nhất của sự kiện
        /// </summary>
        public string MaSuKien { get; set; }

        /// <summary>
        /// Tên sự kiện
        /// </summary>
        public string TenSuKien { get; set; }

        /// <summary>
        /// Mô tả chi tiết về sự kiện
        /// </summary>
        public string MoTa { get; set; }

        /// <summary>
        /// Mã CLB tổ chức sự kiện
        /// </summary>
        public string MaCLB { get; set; }

        /// <summary>
        /// Tên CLB tổ chức
        /// </summary>
        public string TenCLB { get; set; }

        /// <summary>
        /// Địa điểm tổ chức sự kiện
        /// </summary>
        public string DiaDiem { get; set; }

        /// <summary>
        /// Thời gian bắt đầu sự kiện
        /// </summary>
        public DateTime ThoiGianBatDau { get; set; }

        /// <summary>
        /// Thời gian kết thúc sự kiện
        /// </summary>
        public DateTime ThoiGianKetThuc { get; set; }

        /// <summary>
        /// Số lượng người tham gia tối đa
        /// </summary>
        public int SoLuongToiDa { get; set; }

        /// <summary>
        /// Số lượng người đã đăng ký
        /// </summary>
        public int SoLuongDaDangKy { get; set; }

        /// <summary>
        /// Trạng thái sự kiện (SắpDiễnRa, ĐangDiễnRa, ĐãKếtThúc, ĐãHủy)
        /// </summary>
        public string TrangThai { get; set; }

        /// <summary>
        /// Loại sự kiện (HộiThảo, VănHóaNghệThuật, ThểThao, HọcThuật)
        /// </summary>
        public string LoaiSuKien { get; set; }

        /// <summary>
        /// Ảnh sự kiện (đường dẫn)
        /// </summary>
        public string AnhSuKien { get; set; }

        /// <summary>
        /// Ngày tạo sự kiện
        /// </summary>
        public DateTime NgayTao { get; set; }

        /// <summary>
        /// Còn chỗ hay không (tính từ SoLuongToiDa - SoLuongDaDangKy)
        /// </summary>
        public bool ConChoTrong => SoLuongDaDangKy < SoLuongToiDa;

        /// <summary>
        /// Số chỗ còn lại
        /// </summary>
        public int SoChoConLai => SoLuongToiDa - SoLuongDaDangKy;

        public SuKien()
        {
            MaSuKien = Guid.NewGuid().ToString();
            NgayTao = DateTime.Now;
            TrangThai = "SắpDiễnRa";
            SoLuongDaDangKy = 0;
        }
    }
}
