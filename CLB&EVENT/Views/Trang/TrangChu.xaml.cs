using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CLB_EVENT.Models;
using CLB_EVENT.Services;

namespace CLB_EVENT
{
    /// <summary>
    /// Trang chủ: hiển thị lời chào, thống kê và tin tức tổng quan
    /// </summary>
    public partial class TrangChu : UserControl
    {
        // Các dịch vụ cần dùng
        private readonly DichVuTinTuc _dichVuTinTuc;
        private readonly DichVuSuKien _dichVuSuKien;
        private readonly DichVuCLB _dichVuCLB;

        public TrangChu()
        {
            InitializeComponent();
            _dichVuTinTuc = new DichVuTinTuc();
            _dichVuSuKien = new DichVuSuKien();
            _dichVuCLB = new DichVuCLB();

            // Tải dữ liệu khi trang được khởi tạo
            HienThiLoiChao();
            TaiDuLieuTrangChu();
        }

        /// <summary>
        /// Hiển thị lời chào theo thời gian trong ngày
        /// </summary>
        private void HienThiLoiChao()
        {
            // Lấy thông tin người dùng đang đăng nhập
            var nguoiDung = DichVuNguoiDung.NguoiDungHienTai;
            if (nguoiDung != null)
                tbTenNguoiDung.Text = nguoiDung.HoTen;

            // Xác định buổi trong ngày để chào
            int gio = DateTime.Now.Hour;
            if (gio < 12)
                tbLoiChao.Text = "Chào buổi sáng,";
            else if (gio < 17)
                tbLoiChao.Text = "Chào buổi chiều,";
            else
                tbLoiChao.Text = "Chào buổi tối,";

            // Hiển thị ngày tháng hiện tại (định dạng tiếng Việt)
            string[] thuTrongTuan = { "Chủ Nhật", "Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy" };
            string tenThu = thuTrongTuan[(int)DateTime.Now.DayOfWeek];
            tbNgayHomNay.Text = string.Format("{0}, ngày {1:dd/MM/yyyy}", tenThu, DateTime.Now);
        }

        /// <summary>
        /// Tải tất cả dữ liệu hiển thị trên trang chủ
        /// </summary>
        private void TaiDuLieuTrangChu()
        {
            TaiTinCLB();
            TaiHoatDong();
            TaiSuKienSapToi();
            TaiThongKe();
        }

        public void LamMoi()
        {
            HienThiLoiChao();
            TaiDuLieuTrangChu();
        }

        /// <summary>
        /// Tải danh sách tin tức CLB mới nhất
        /// </summary>
        private void TaiTinCLB()
        {
            // Lấy 5 tin tức mới nhất để hiển thị
            var danhSachTin = _dichVuTinTuc.LayDanhSachTinTuc()
                .OrderByDescending(t => t.NgayDang)
                .Take(5)
                .Select(t => new
                {
                    MaTinTuc = t.MaTinTuc,
                    TieuDe = t.TieuDe,
                    NgayDangHienThi = "📅 " + t.NgayDang.ToString("dd/MM/yyyy")
                })
                .ToList();

            dsTinCLB.ItemsSource = danhSachTin;
        }

        private void TaiHoatDong()
        {
            var tin = _dichVuTinTuc.LayTinTucTheoDanhMuc("HoạtĐộngSinhViên")
                .OrderByDescending(t => t.NgayDang).Take(3).ToList();
            dsHoatDong.ItemsSource = tin;
            borderTrongHoatDong.Visibility = tin.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TheTin_Click(object sender, MouseButtonEventArgs e)
        {
            TrangTinTuc.HienThiTin(Window.GetWindow(this), (sender as FrameworkElement)?.Tag as string);
        }

        /// <summary>
        /// Tải danh sách sự kiện sắp diễn ra
        /// </summary>
        private void TaiSuKienSapToi()
        {
            // Lấy các sự kiện trong tương lai
            var suKienSapToi = _dichVuSuKien.LayDanhSachSuKienSapToi()
                .Take(4)
                .Select(sk => new
                {
                    TenSuKien = sk.TenSuKien,
                    TenCLB = sk.TenCLB,
                    DiaDiem = "📍 " + sk.DiaDiem,
                    NgaySuKien = sk.ThoiGianBatDau.Day.ToString(),
                    ThangSuKien = "T" + sk.ThoiGianBatDau.Month
                })
                .ToList();

            if (suKienSapToi.Count > 0)
            {
                thongBaoTrongSuKien.Visibility = Visibility.Collapsed;
                dsSuKienSapToi.ItemsSource = suKienSapToi;
            }
            else
            {
                thongBaoTrongSuKien.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Tải thống kê tổng quan
        /// </summary>
        private void TaiThongKe()
        {
            // Tổng số CLB đang hoạt động
            int soCLB = _dichVuCLB.LayDanhSachCLB().Count;
            tbTongCLB.Text = soCLB.ToString();

            // Tổng số sự kiện sắp tới
            int soSuKien = _dichVuSuKien.LayDanhSachSuKienSapToi().Count;
            tbTongSuKien.Text = soSuKien.ToString();
        }

        /// <summary>
        /// Xử lý khi click vào thẻ chức năng nhanh - điều hướng đến trang tương ứng
        /// </summary>
        private void TheChucNang_Click(object sender, MouseButtonEventArgs e)
        {
            // Lấy tag để biết cần chuyển đến trang nào
            var viTri = sender as FrameworkElement;
            if (viTri == null) return;

            string tenTrang = viTri.Tag?.ToString() ?? string.Empty;

            // Tìm cửa sổ cha là CuaSoChinh và yêu cầu chuyển trang
            var cuaSoChinh = Window.GetWindow(this) as CuaSoChinh;
            if (cuaSoChinh == null) return;

            // Gọi phương thức chuyển trang trong cửa sổ chính
            cuaSoChinh.ChuyenTrangTuTrangChu(tenTrang);
        }
    }
}
