using System.Windows;
using System.Windows.Media;
using CLB_EVENT.Models;
using CLB_EVENT.Services;

namespace CLB_EVENT
{
    /// <summary>
    /// Màn hình đăng ký tài khoản mới cho sinh viên
    /// </summary>
    public partial class CuaSoDangKy : Window
    {
        // Dịch vụ quản lý người dùng
        private readonly DichVuNguoiDung _dichVuNguoiDung;

        public CuaSoDangKy()
        {
            InitializeComponent();
            _dichVuNguoiDung = new DichVuNguoiDung();
        }

        /// <summary>
        /// Xử lý khi nhấn nút Đăng Ký
        /// </summary>
        private void NutDangKy_Click(object sender, RoutedEventArgs e)
        {
            // Lấy thông tin từ form
            string hoTen = tbHoTen.Text?.Trim() ?? string.Empty;
            string tenDangNhap = tbTenDangNhap.Text?.Trim() ?? string.Empty;
            string email = tbEmail.Text?.Trim() ?? string.Empty;
            string soDienThoai = tbSoDienThoai.Text?.Trim() ?? string.Empty;
            string lopHoc = tbLopHoc.Text?.Trim() ?? string.Empty;
            string matKhau = pbMatKhau.Password ?? string.Empty;
            string xacNhanMatKhau = pbXacNhanMatKhau.Password ?? string.Empty;

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(hoTen))
            {
                HienThiLoi("Vui lòng nhập họ và tên!");
                return;
            }

            if (string.IsNullOrWhiteSpace(tenDangNhap))
            {
                HienThiLoi("Vui lòng nhập tên đăng nhập!");
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                HienThiLoi("Vui lòng nhập email!");
                return;
            }

            if (string.IsNullOrWhiteSpace(matKhau))
            {
                HienThiLoi("Vui lòng nhập mật khẩu!");
                return;
            }

            // Kiểm tra mật khẩu đủ mạnh
            if (!DichVuMaHoa.KiemTraDoManhMatKhau(matKhau))
            {
                HienThiLoi("Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường và số!");
                return;
            }

            // Kiểm tra xác nhận mật khẩu trùng khớp
            if (matKhau != xacNhanMatKhau)
            {
                HienThiLoi("Mật khẩu xác nhận không khớp!");
                return;
            }

            // Tạo đối tượng người dùng mới
            var nguoiDungMoi = new NguoiDung
            {
                HoTen = hoTen,
                TenDangNhap = tenDangNhap,
                Email = email,
                SoDienThoai = soDienThoai,
                LopHoc = lopHoc
            };

            // Gọi dịch vụ đăng ký (mật khẩu sẽ được mã hóa SHA-256 bên trong)
            var ketQua = _dichVuNguoiDung.DangKyTaiKhoan(nguoiDungMoi, matKhau);

            if (ketQua.ThanhCong)
            {
                HienThiThanhCong("Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.");
                XoaForm();
            }
            else
            {
                HienThiLoi(ketQua.ThongBao);
            }
        }

        /// <summary>
        /// Hiển thị thông báo lỗi màu đỏ
        /// </summary>
        private void HienThiLoi(string thongBao)
        {
            tbThongBao.Text = "❌ " + thongBao;
            tbThongBao.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 40, 40));
            borderThongBao.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 235, 238));
            borderThongBao.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hiển thị thông báo thành công màu xanh
        /// </summary>
        private void HienThiThanhCong(string thongBao)
        {
            tbThongBao.Text = "✅ " + thongBao;
            tbThongBao.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(27, 94, 32));
            borderThongBao.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(232, 245, 233));
            borderThongBao.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Xóa form sau khi đăng ký thành công
        /// </summary>
        private void XoaForm()
        {
            tbHoTen.Clear();
            tbTenDangNhap.Clear();
            tbEmail.Clear();
            tbSoDienThoai.Clear();
            tbLopHoc.Clear();
            pbMatKhau.Clear();
            pbXacNhanMatKhau.Clear();
        }

        /// <summary>
        /// Quay lại màn hình đăng nhập
        /// </summary>
        private void NutQuayLai_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
