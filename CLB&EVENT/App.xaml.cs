using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CLB_EVENT.DuLieu;
using MySqlConnector;

namespace CLB_EVENT
{
    /// <summary>
    /// Lớp khởi động ứng dụng quản lý CLB và Sự Kiện
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += (sender, e) =>
            {
                if (!(e.Exception is MySqlException)) return;
                MessageBox.Show(KhoDuLieuAiven.ThongBaoLoi(e.Exception), "Kết nối Aiven", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true;
            };
        }
        /// <summary>
        /// Xử lý sự kiện khởi động ứng dụng - mở màn hình đăng nhập
        /// </summary>
        private async void App_KhoiDong(object sender, StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var panel = new StackPanel { Margin = new Thickness(24) };
            panel.Children.Add(new TextBlock { Text = "Đang kết nối dữ liệu Aiven…", FontSize = 18, Margin = new Thickness(0, 0, 0, 16) });
            panel.Children.Add(new ProgressBar { IsIndeterminate = true, Height = 8 });
            var loading = new Window { Title = "CLB & Sự kiện", Width = 400, Height = 145, ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen, Content = panel };
            loading.Show();
            try
            {
                await Task.Run(() => KhoDuLieuAiven.KhoiTao());
                var login = new CuaSoDangNhap();
                MainWindow = login;
                login.Show();
                loading.Close();
                ShutdownMode = ShutdownMode.OnLastWindowClose;
            }
            catch (Exception ex)
            {
                loading.Close();
                MessageBox.Show(KhoDuLieuAiven.ThongBaoLoi(ex), "Không thể mở dữ liệu Aiven", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }
    }
}
