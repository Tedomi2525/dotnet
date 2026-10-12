using System.Windows;
using System.Windows.Input;
using CLB_EVENT.Services;

namespace CLB_EVENT
{
    /// <summary>
    /// Màn hình đăng nhập vào hệ thống quản lý CLB và Sự Kiện
    /// </summary>
    public partial class CuaSoDangNhap : Window
    {
        // Dịch vụ xử lý đăng nhập
        private readonly DichVuNguoiDung _dichVuNguoiDung;

        public CuaSoDangNhap()
        {
            InitializeComponent();
            _dichVuNguoiDung = new DichVuNguoiDung();
        }

        /// <summary>
        /// Xử lý khi người dùng nhấn nút Đăng Nhập
        /// </summary>
        private void NutDangNhap_Click(object sender, RoutedEventArgs e)
        {
            ThucHienDangNhap();
        }

        /// <summary>
        /// Xử lý khi nhấn Enter trong ô mật khẩu
        /// </summary>
        private void PbMatKhau_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ThucHienDangNhap();
        }

        /// <summary>
        /// Thực hiện logic đăng nhập: kiểm tra thông tin và mở cửa sổ chính
        /// </summary>
        private void ThucHienDangNhap()
        {
            // Lấy thông tin người dùng nhập vào
            string tenDangNhap = tbTenDangNhap.Text?.Trim() ?? string.Empty;
            string matKhau = pbMatKhau.Password ?? string.Empty;

            // Kiểm tra không để trống
            if (string.IsNullOrWhiteSpace(tenDangNhap))
            {
                HienThiLoi("Vui lòng nhập tên đăng nhập!");
                tbTenDangNhap.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(matKhau))
            {
                HienThiLoi("Vui lòng nhập mật khẩu!");
                pbMatKhau.Focus();
                return;
            }

            // Gọi dịch vụ đăng nhập (mật khẩu được mã hóa SHA-256 bên trong)
            var nguoiDung = _dichVuNguoiDung.DangNhap(tenDangNhap, matKhau);

            if (nguoiDung == null)
            {
                HienThiLoi("Tên đăng nhập hoặc mật khẩu không chính xác, hoặc tài khoản đã bị khóa!");
                pbMatKhau.Clear();
                pbMatKhau.Focus();
                return;
            }

            // Đăng nhập thành công - mở cửa sổ chính
            var cuaSoChinh = new CuaSoChinh();
            cuaSoChinh.Show();
            this.Close();
        }

        /// <summary>
        /// Hiển thị thông báo lỗi trên giao diện
        /// </summary>
        private void HienThiLoi(string thongBao)
        {
            tbThongBaoLoi.Text = thongBao;
            borderThongBaoLoi.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Xử lý khi nhấn nút chuyển sang màn hình Đăng Ký
        /// </summary>
        private void NutChuyenDangKy_Click(object sender, RoutedEventArgs e)
        {
            var cuaSoDangKy = new CuaSoDangKy();
            cuaSoDangKy.Owner = this;
            cuaSoDangKy.ShowDialog();
        }
    }
}
