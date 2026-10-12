using System.Windows;
using System.Windows.Controls;
using CLB_EVENT.Models;
using CLB_EVENT.Services;

namespace CLB_EVENT
{
    /// <summary>
    /// Cửa sổ chính của ứng dụng - chứa sidebar điều hướng và nội dung trang
    /// </summary>
    public partial class CuaSoChinh : Window
    {
        // Dịch vụ quản lý người dùng
        private readonly DichVuNguoiDung _dichVuNguoiDung;

        // Các trang UserControl được tạo một lần và tái sử dụng
        private TrangChu _trangChu;
        private TrangTinTuc _trangTinTuc;
        private TrangDangKySuKien _trangDangKySuKien;
        private TrangThoiGianSuKien _trangThoiGianSuKien;
        private TrangQuanLyCLB _trangQuanLyCLB;
        private TrangDangKyCLBForm _trangDangKyCLBForm;

        public CuaSoChinh()
        {
            InitializeComponent();
            _dichVuNguoiDung = new DichVuNguoiDung();

            // Hiển thị thông tin người dùng đang đăng nhập
            HienThiThongTinNguoiDung();

            // Điều chỉnh menu theo vai trò
            DieuChinhMenuTheoVaiTro();

            // Mở trang chủ khi khởi động
            ChuyenDenTrangChu();
        }

        /// <summary>
        /// Hiển thị thông tin người dùng đang đăng nhập lên giao diện
        /// </summary>
        private void HienThiThongTinNguoiDung()
        {
            var nguoiDung = DichVuNguoiDung.NguoiDungHienTai;
            if (nguoiDung == null) return;

            // Hiển thị tên trên sidebar và header
            tbTenNguoiDung.Text = nguoiDung.HoTen;
            tbTenTrenHeader.Text = nguoiDung.HoTen;

            // Hiển thị vai trò tiếng Việt
            switch (nguoiDung.VaiTro)
            {
                case "QuanLyAdmin":
                    tbVaiTro.Text = "Quản trị viên";
                    break;
                case "QuanLyCLB":
                    tbVaiTro.Text = "Quản lý CLB";
                    break;
                default:
                    tbVaiTro.Text = "Sinh viên";
                    break;
            }
        }

        /// <summary>
        /// Hiển thị hoặc ẩn menu quản lý CLB tùy theo vai trò người dùng
        /// </summary>
        private void DieuChinhMenuTheoVaiTro()
        {
            var nguoiDung = DichVuNguoiDung.NguoiDungHienTai;
            if (nguoiDung == null) return;

            // Chỉ Admin và Quản lý CLB mới thấy menu quản lý
            bool laQuanLy = nguoiDung.VaiTro == "QuanLyAdmin" || nguoiDung.VaiTro == "QuanLyCLB";

            if (laQuanLy)
            {
                tbNhanQuanLy.Visibility = Visibility.Visible;
                rbQuanLyCLB.Visibility = Visibility.Visible;
                rbDuyetDonCLB.Visibility = Visibility.Visible;
                rbQuanLySuKien.Visibility = Visibility.Visible;
                rbQuanLyTinTuc.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Xử lý khi người dùng chọn mục điều hướng trên sidebar
        /// </summary>
        private void DieuHuong_Checked(object sender, RoutedEventArgs e)
        {
            // Checked can fire while InitializeComponent is still building the XAML tree.
            if (!IsInitialized || vungNoiDung == null)
                return;

            // Xác định trang cần chuyển đến dựa vào RadioButton nào được chọn
            if (sender == rbTrangChu)
                ChuyenDenTrangChu();
            else if (sender == rbTinTuc)
                ChuyenDenTrangTinTuc();
            else if (sender == rbDangKySuKien)
                ChuyenDenTrangDangKySuKien();
            else if (sender == rbThoiGianSuKien)
                ChuyenDenTrangThoiGianSuKien();
            else if (sender == rbQuanLyCLB)
                ChuyenDenTrangQuanLyCLB();
            else if (sender == rbDangKyCLB)
                ChuyenDenTrangDangKyCLBForm();
            else if (DichVuPhanQuyen.LaQuanLy && sender == rbDuyetDonCLB)
                vungNoiDung.Content = new TrangDuyetDonCLB();
            else if (DichVuPhanQuyen.LaQuanLy && sender == rbQuanLySuKien)
                vungNoiDung.Content = new TrangQuanLySuKien();
            else if (DichVuPhanQuyen.LaQuanLy && sender == rbQuanLyTinTuc)
                vungNoiDung.Content = new TrangQuanLyTinTuc();
        }

        /// <summary>Chuyển đến trang chủ</summary>
        private void ChuyenDenTrangChu()
        {
            if (_trangChu == null)
                _trangChu = new TrangChu();
            else _trangChu.LamMoi();
            vungNoiDung.Content = _trangChu;
        }

        /// <summary>Chuyển đến trang tin tức</summary>
        private void ChuyenDenTrangTinTuc()
        {
            if (_trangTinTuc == null)
                _trangTinTuc = new TrangTinTuc();
            else _trangTinTuc.LamMoi();
            vungNoiDung.Content = _trangTinTuc;
        }

        /// <summary>Chuyển đến trang đăng ký sự kiện</summary>
        private void ChuyenDenTrangDangKySuKien()
        {
            if (_trangDangKySuKien == null)
                _trangDangKySuKien = new TrangDangKySuKien();
            else _trangDangKySuKien.LamMoi();
            vungNoiDung.Content = _trangDangKySuKien;
        }

        /// <summary>Chuyển đến trang thời gian sự kiện</summary>
        private void ChuyenDenTrangThoiGianSuKien()
        {
            if (_trangThoiGianSuKien == null)
                _trangThoiGianSuKien = new TrangThoiGianSuKien();
            else _trangThoiGianSuKien.LamMoi();
            vungNoiDung.Content = _trangThoiGianSuKien;
        }

        /// <summary>Chuyển đến trang quản lý CLB</summary>
        private void ChuyenDenTrangQuanLyCLB()
        {
            if (!DichVuPhanQuyen.LaQuanLy) return;
            if (_trangQuanLyCLB == null)
                _trangQuanLyCLB = new TrangQuanLyCLB();
            else _trangQuanLyCLB.LamMoi();
            vungNoiDung.Content = _trangQuanLyCLB;
        }

        /// <summary>Chuyển đến form đăng ký CLB</summary>
        private void ChuyenDenTrangDangKyCLBForm()
        {
            // Tạo lại mỗi lần để reset form
            _trangDangKyCLBForm = new TrangDangKyCLBForm();
            vungNoiDung.Content = _trangDangKyCLBForm;
        }

        /// <summary>
        /// Xử lý khi nhấn nút đăng xuất - quay lại màn hình đăng nhập
        /// </summary>
        private void NutDangXuat_Click(object sender, RoutedEventArgs e)
        {
            var ketQua = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận đăng xuất",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (ketQua == MessageBoxResult.Yes)
            {
                // Đăng xuất và quay về màn hình đăng nhập
                _dichVuNguoiDung.DangXuat();
                var cuaSoDangNhap = new CuaSoDangNhap();
                cuaSoDangNhap.Show();
                this.Close();
            }
        }

        /// <summary>
        /// Phương thức cho phép TrangChu gọi để chuyển đến các trang khác
        /// </summary>
        public void ChuyenTrangTuTrangChu(string tenTrang)
        {
            switch (tenTrang)
            {
                case "DangKySuKien":
                    rbDangKySuKien.IsChecked = true;
                    break;
                case "ThoiGianSuKien":
                    rbThoiGianSuKien.IsChecked = true;
                    break;
                case "TinTuc":
                    rbTinTuc.IsChecked = true;
                    break;
                case "DangKyCLB":
                    rbDangKyCLB.IsChecked = true;
                    break;
                default:
                    rbTrangChu.IsChecked = true;
                    break;
            }
        }

        private void TimKiemChung_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;
            rbTinTuc.IsChecked = true;
            ChuyenDenTrangTinTuc();
            _trangTinTuc.TimKiem(tbTimKiem.Text?.Trim() ?? string.Empty);
        }
    }
}
