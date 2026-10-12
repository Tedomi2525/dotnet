using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CLB_EVENT.Services;

namespace CLB_EVENT
{
    public partial class TrangTinTuc : UserControl
    {
        private readonly DichVuTinTuc _dichVuTinTuc;
        public TrangTinTuc()
        {
            InitializeComponent();
            _dichVuTinTuc = new DichVuTinTuc();
            LamMoi();
        }

        public void LamMoi()
        {
            if (_dichVuTinTuc == null) return;
            if (dpTuNgay.SelectedDate.HasValue && dpDenNgay.SelectedDate.HasValue &&
                dpTuNgay.SelectedDate.Value.Date > dpDenNgay.SelectedDate.Value.Date)
            {
                dsTinTrai.ItemsSource = null;
                dsTinPhai.ItemsSource = null;
                tbKetQua.Text = "Ngày bắt đầu phải trước hoặc bằng ngày kết thúc.";
                return;
            }
            var category = rbThongBao.IsChecked == true ? "ThôngBáo" :
                rbSuKien.IsChecked == true ? "SựKiện" :
                rbHoatDong.IsChecked == true ? "HoạtĐộngSinhViên" :
                rbCLB.IsChecked == true ? "CLB" : null;
            var result = _dichVuTinTuc.TimKiemTinTuc(tbTimKiem.Text?.Trim())
                .Where(t => (category == null || t.DanhMuc == category) &&
                    (!dpTuNgay.SelectedDate.HasValue || t.NgayDang.Date >= dpTuNgay.SelectedDate.Value.Date) &&
                    (!dpDenNgay.SelectedDate.HasValue || t.NgayDang.Date <= dpDenNgay.SelectedDate.Value.Date))
                .OrderByDescending(t => t.NgayDang).ToList();
            int half = (result.Count + 1) / 2;
            dsTinTrai.ItemsSource = result.Take(half).ToList();
            dsTinPhai.ItemsSource = result.Skip(half).ToList();
            tbKetQua.Text = result.Count == 0 ? "Không có bài tin phù hợp." : result.Count + " bài tin • Bấm vào bài để đọc nội dung.";
        }

        public void TimKiem(string tuKhoa) { tbTimKiem.Text = tuKhoa; LamMoi(); }
        private void NutTimKiem_Click(object sender, RoutedEventArgs e) => LamMoi();
        private void TimKiem_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) LamMoi(); }
        private void LocDanhMuc_Checked(object sender, RoutedEventArgs e) { if (IsInitialized) LamMoi(); }
        private void NutXoaLoc_Click(object sender, RoutedEventArgs e)
        {
            tbTimKiem.Clear();
            dpTuNgay.SelectedDate = null;
            dpDenNgay.SelectedDate = null;
            rbToanBo.IsChecked = true;
            LamMoi();
        }
        private void NutDocTin_Click(object sender, RoutedEventArgs e)
        {
            HienThiTin(Window.GetWindow(this), (sender as Button)?.Tag as string);
        }

        public static void HienThiTin(Window owner, string maTin)
        {
            var tin = new DichVuTinTuc().DocTin(maTin);
            if (tin == null) return;
            var panel = new StackPanel { Margin = new Thickness(24) };
            panel.Children.Add(new TextBlock { Text = tin.TieuDe, FontSize = 22, FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap });
            panel.Children.Add(new TextBlock { Text = $"{tin.TacGia} • {tin.NgayDang:dd/MM/yyyy} • {tin.LuotXem} lượt xem", Foreground = System.Windows.Media.Brushes.Gray, Margin = new Thickness(0, 12, 0, 20), TextWrapping = TextWrapping.Wrap });
            panel.Children.Add(new TextBlock { Text = tin.NoiDung, FontSize = 15, TextWrapping = TextWrapping.Wrap });
            new Window { Title = "Đọc tin", Width = 720, Height = 540, MinWidth = 420, MinHeight = 300,
                Owner = owner, WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Content = panel } }.ShowDialog();
        }
    }
}
