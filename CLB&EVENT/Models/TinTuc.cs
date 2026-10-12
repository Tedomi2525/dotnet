using System;

namespace CLB_EVENT.Models
{
    /// <summary>
    /// Lớp đại diện cho một bài tin tức/thông báo trong hệ thống
    /// </summary>
    public class TinTuc
    {
        /// <summary>
        /// Mã định danh duy nhất của tin tức
        /// </summary>
        public string MaTinTuc { get; set; }

        /// <summary>
        /// Tiêu đề bài tin
        /// </summary>
        public string TieuDe { get; set; }

        /// <summary>
        /// Nội dung đầy đủ bài tin
        /// </summary>
        public string NoiDung { get; set; }

        /// <summary>
        /// Tóm tắt ngắn gọn của bài tin
        /// </summary>
        public string TomTat { get; set; }

        /// <summary>
        /// Tác giả bài tin
        /// </summary>
        public string TacGia { get; set; }

        /// <summary>
        /// Ngày đăng bài
        /// </summary>
        public DateTime NgayDang { get; set; }

        /// <summary>
        /// Danh mục bài tin (ThôngBáo, SựKiện, HoạtĐộngSinhViên, CLB)
        /// </summary>
        public string DanhMuc { get; set; }

        /// <summary>
        /// Mã CLB liên quan (nếu có)
        /// </summary>
        public string MaCLB { get; set; }

        /// <summary>
        /// Ảnh minh họa (đường dẫn)
        /// </summary>
        public string AnhMinhHoa { get; set; }

        /// <summary>
        /// Số lượt xem
        /// </summary>
        public int LuotXem { get; set; }

        /// <summary>
        /// Trạng thái (ĐãDuyệt, ChờDuyệt, ĐãẨn)
        /// </summary>
        public string TrangThai { get; set; }

        public TinTuc()
        {
            MaTinTuc = Guid.NewGuid().ToString();
            NgayDang = DateTime.Now;
            TrangThai = "ĐãDuyệt";
            LuotXem = 0;
        }
    }
}
