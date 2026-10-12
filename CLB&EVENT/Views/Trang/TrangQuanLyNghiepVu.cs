using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using CLB_EVENT.Models;
using CLB_EVENT.Services;

namespace CLB_EVENT
{
    internal static class GiaoDienNghiepVu
    {
        public static Button Nut(string text, RoutedEventHandler click)
        {
            var button = new Button { Content = text, Padding = new Thickness(12, 8, 12, 8), Margin = new Thickness(0, 0, 8, 0) };
            button.Click += click;
            return button;
        }
        public static DataGrid Bang(params string[] columns)
        {
            var grid = new DataGrid { AutoGenerateColumns = false, IsReadOnly = true, CanUserAddRows = false,
                SelectionMode = DataGridSelectionMode.Single, SelectionUnit = DataGridSelectionUnit.FullRow,
                Margin = new Thickness(0, 12, 0, 12), Background = Brushes.White,
                MinRowHeight = 32, HeadersVisibility = DataGridHeadersVisibility.Column,
                GridLinesVisibility = DataGridGridLinesVisibility.Horizontal };
            for (int i = 0; i < columns.Length; i += 2)
            {
                var property = columns[i + 1];
                var binding = new Binding(property);
                if (property.StartsWith("ThoiGian") || property.StartsWith("Ngay")) binding.StringFormat = "dd/MM/yyyy HH:mm";
                if (property == "TrangThai" || property == "DanhMuc") binding.Converter = new NhanTrangThai();
                bool title = property == "TenSuKien" || property == "TieuDe";
                grid.Columns.Add(new DataGridTextColumn { Header = columns[i], Binding = binding,
                    MinWidth = title ? 200 : property == "TenCLB" ? 160 : 110,
                    Width = new DataGridLength(title ? 2 : 1, DataGridLengthUnitType.Star) });
            }
            return grid;
        }
        public static Grid Trang(string title)
        {
            var grid = new Grid { Margin = new Thickness(25, 18, 25, 18) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.Children.Add(new TextBlock { Text = title, FontSize = 21, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 16) });
            return grid;
        }
        public static void Them(Grid grid, UIElement element, int row) { Grid.SetRow(element, row); grid.Children.Add(element); }
        public static bool XacNhan(DependencyObject owner, string text) =>
            MessageBox.Show(Window.GetWindow(owner), text, "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    public class TrangDuyetDonCLB : UserControl
    {
        private readonly DichVuCLB _clubs = new DichVuCLB();
        private readonly DataGrid _bang;
        private readonly ComboBox _clb = new ComboBox { Width = 250, DisplayMemberPath = "TenCLB", Margin = new Thickness(0, 0, 12, 0) };
        private readonly CheckBox _choDuyet = new CheckBox { Content = "Chỉ đơn chờ duyệt", IsChecked = true, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 12, 0) };
        private readonly TextBlock _chiTiet = new TextBlock { TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 8, 0, 0) };
        private readonly Button _duyet, _tuChoi;
        public TrangDuyetDonCLB()
        {
            Background = new SolidColorBrush(Color.FromRgb(240, 242, 245));
            var grid = GiaoDienNghiepVu.Trang("Duyệt đơn đăng ký CLB");
            var toolbar = new WrapPanel();
            toolbar.Children.Add(_clb); toolbar.Children.Add(_choDuyet);
            toolbar.Children.Add(GiaoDienNghiepVu.Nut("Làm mới", (s,e) => LamMoi()));
            _duyet = GiaoDienNghiepVu.Nut("Chấp nhận", (s,e) => XuLy(true));
            _tuChoi = GiaoDienNghiepVu.Nut("Từ chối", (s,e) => XuLy(false));
            toolbar.Children.Add(_duyet); toolbar.Children.Add(_tuChoi);
            GiaoDienNghiepVu.Them(grid, toolbar, 1);
            _bang = GiaoDienNghiepVu.Bang("Họ tên", "HoTen", "CLB", "TenCLB", "Lớp", "LopHoc", "Email", "Email", "Trạng thái", "TrangThai");
            _bang.SelectionChanged += (s,e) => ChonDon();
            GiaoDienNghiepVu.Them(grid, _bang, 2);
            GiaoDienNghiepVu.Them(grid, _chiTiet, 3);
            Content = grid;
            _clb.SelectionChanged += (s,e) => TaiDon();
            _choDuyet.Checked += (s,e) => TaiDon(); _choDuyet.Unchecked += (s,e) => TaiDon();
            LamMoi();
        }
        public void LamMoi()
        {
            string ma = (_clb.SelectedItem as CauLacBo)?.MaCLB;
            var choices = _clubs.LayCLBDuocQuanLy();
            choices.Insert(0, new CauLacBo { MaCLB = null, TenCLB = "Tất cả CLB được quản lý" });
            _clb.ItemsSource = choices;
            _clb.SelectedItem = choices.FirstOrDefault(c => c.MaCLB == ma) ?? choices[0];
            TaiDon();
        }
        private void TaiDon()
        {
            if (_bang == null) return;
            var ma = (_clb.SelectedItem as CauLacBo)?.MaCLB;
            var don = _clubs.LayCLBDuocQuanLy().Where(c => ma == null || c.MaCLB == ma)
                .SelectMany(c => _clubs.LayDanhSachDangKyTheoCLB(c.MaCLB))
                .Where(d => _choDuyet.IsChecked != true || d.TrangThai == "ChờDuyệt")
                .OrderBy(d => d.NgayDangKy).ToList();
            _bang.ItemsSource = don;
            ChonDon();
        }
        private void ChonDon()
        {
            var don = _bang.SelectedItem as DangKyCLB;
            _duyet.IsEnabled = _tuChoi.IsEnabled = don != null && don.TrangThai == "ChờDuyệt" &&
                DichVuPhanQuyen.QuanLyCLB(_clubs.TimCLBTheoMa(don.MaCLB));
            _chiTiet.Text = don == null ? "Chọn một đơn để xem lý do, kỹ năng và xét duyệt." :
                $"Lý do: {don.LyDo}\nKỹ năng: {don.KyNang}\nThời gian rảnh: {don.ThoiGianRanh}\nĐiện thoại: {don.SoDienThoai} • Ngày nộp: {don.NgayDangKy:dd/MM/yyyy HH:mm}";
        }
        private void XuLy(bool duyet)
        {
            var don = _bang.SelectedItem as DangKyCLB;
            if (don == null) return;
            if (!GiaoDienNghiepVu.XacNhan(this, (duyet ? "Chấp nhận" : "Từ chối") + " đơn của " + don.HoTen + "?")) return;
            var result = _clubs.XuLyDonDangKy(don.MaDangKy, duyet);
            TaiDon();
            _chiTiet.Text = result.ThongBao;
        }
    }

    public class TrangQuanLySuKien : UserControl
    {
        private readonly DichVuSuKien _events = new DichVuSuKien();
        private readonly DataGrid _bang, _nguoiDangKy;
        private readonly TextBlock _thongBao = new TextBlock { TextWrapping = TextWrapping.Wrap };
        private readonly Button _them, _sua, _huy;
        public TrangQuanLySuKien()
        {
            Background = new SolidColorBrush(Color.FromRgb(240, 242, 245));
            var grid = GiaoDienNghiepVu.Trang("Quản lý sự kiện");
            var toolbar = new WrapPanel();
            _them = GiaoDienNghiepVu.Nut("Thêm sự kiện", (s,e) => MoForm(null));
            _sua = GiaoDienNghiepVu.Nut("Sửa sự kiện", (s,e) => MoForm(_bang.SelectedItem as SuKien));
            _huy = GiaoDienNghiepVu.Nut("Hủy sự kiện", (s,e) => Huy());
            toolbar.Children.Add(_them); toolbar.Children.Add(_sua); toolbar.Children.Add(_huy);
            toolbar.Children.Add(GiaoDienNghiepVu.Nut("Làm mới", (s,e) => LamMoi()));
            GiaoDienNghiepVu.Them(grid, toolbar, 1);
            _bang = GiaoDienNghiepVu.Bang("Tên sự kiện", "TenSuKien", "CLB", "TenCLB", "Bắt đầu", "ThoiGianBatDau", "Đăng ký", "SoLuongDaDangKy", "Tối đa", "SoLuongToiDa", "Trạng thái", "TrangThai");
            _bang.SelectionChanged += (s,e) => ChonSuKien();
            GiaoDienNghiepVu.Them(grid, _bang, 2);
            var detail = new StackPanel();
            detail.Children.Add(_thongBao);
            detail.Children.Add(new TextBlock { Text = "Danh sách người đăng ký", FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 12, 0, 0) });
            _nguoiDangKy = GiaoDienNghiepVu.Bang("Họ tên", "HoTen", "Email", "Email", "Điện thoại", "SoDienThoai", "Ghi chú", "GhiChu");
            _nguoiDangKy.Height = 150;
            detail.Children.Add(_nguoiDangKy);
            GiaoDienNghiepVu.Them(grid, detail, 3);
            Content = grid; LamMoi();
        }
        public void LamMoi()
        {
            _bang.ItemsSource = _events.LaySuKienDuocQuanLy();
            _them.IsEnabled = new DichVuCLB().LayCLBDuocQuanLy().Count > 0;
            ChonSuKien();
        }
        private void ChonSuKien()
        {
            var sk = _bang.SelectedItem as SuKien;
            _sua.IsEnabled = _huy.IsEnabled = sk != null && sk.TrangThai == "SắpDiễnRa" &&
                DichVuPhanQuyen.QuanLyCLB(new DichVuCLB().TimCLBTheoMa(sk.MaCLB));
            _nguoiDangKy.ItemsSource = sk == null ? null : _events.LayDangKyTheoSuKien(sk.MaSuKien);
            _thongBao.Text = sk == null ? "Chọn sự kiện để chỉnh sửa, hủy hoặc xem người đăng ký." :
                $"{sk.DiaDiem} • {sk.ThoiGianBatDau:dd/MM/yyyy HH:mm} – {sk.ThoiGianKetThuc:dd/MM/yyyy HH:mm}\n{sk.MoTa}";
        }
        private void MoForm(SuKien sk)
        {
            if (sk == null && !_them.IsEnabled) return;
            var form = new FormSuKien(sk) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) LamMoi();
        }
        private void Huy()
        {
            var sk = _bang.SelectedItem as SuKien;
            if (sk == null || !GiaoDienNghiepVu.XacNhan(this, "Hủy sự kiện '" + sk.TenSuKien + "'? Các đăng ký liên quan sẽ được cập nhật là sự kiện đã hủy.")) return;
            var result = _events.HuySuKien(sk.MaSuKien);
            LamMoi(); _thongBao.Text = result.ThongBao;
        }
    }

    internal class FormNghiepVu : Window
    {
        protected readonly StackPanel NoiDung = new StackPanel { Margin = new Thickness(24) };
        protected readonly TextBlock Loi = new TextBlock { Foreground = Brushes.Firebrick, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 12, 0, 12) };
        protected FormNghiepVu(string title)
        {
            Title = title; Width = 600; Height = 700; MinWidth = 420; MinHeight = 350;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Content = new ScrollViewer { Content = NoiDung, Background = Brushes.White, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        }
        protected T Truong<T>(string label, T control) where T : Control
        {
            NoiDung.Children.Add(new TextBlock { Text = label, Margin = new Thickness(0, 10, 0, 5) });
            control.Padding = new Thickness(8); control.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            NoiDung.Children.Add(control); return control;
        }
        protected TextBox Nhap(string label, string value, bool multi = false) =>
            Truong(label, new TextBox { Text = value ?? "", AcceptsReturn = multi,
                TextWrapping = TextWrapping.Wrap, MinHeight = multi ? 90 : 32 });
        protected void NutLuu(Func<(bool ThanhCong, string ThongBao)> save)
        {
            NoiDung.Children.Add(Loi);
            var actions = new WrapPanel();
            actions.Children.Add(GiaoDienNghiepVu.Nut("Lưu", (s,e) => {
                var result = save(); Loi.Text = result.ThongBao;
                if (result.ThanhCong) DialogResult = true;
            }));
            actions.Children.Add(GiaoDienNghiepVu.Nut("Đóng", (s,e) => Close()));
            NoiDung.Children.Add(actions);
        }
    }

    internal class FormSuKien : FormNghiepVu
    {
        public FormSuKien(SuKien cu) : base(cu == null ? "Thêm sự kiện" : "Sửa sự kiện")
        {
            var ten = Nhap("Tên sự kiện *", cu?.TenSuKien);
            var clubs = new DichVuCLB().LayCLBDuocQuanLy();
            var clb = Truong("CLB tổ chức *", new ComboBox { ItemsSource = clubs, DisplayMemberPath = "TenCLB" });
            clb.SelectedItem = clubs.FirstOrDefault(c => c.MaCLB == cu?.MaCLB) ?? clubs.FirstOrDefault();
            var diaDiem = Nhap("Địa điểm *", cu?.DiaDiem);
            var start = cu?.ThoiGianBatDau ?? DateTime.Today.AddDays(1).AddHours(8);
            var end = cu?.ThoiGianKetThuc ?? start.AddHours(2);
            var ngayBD = Truong("Ngày bắt đầu *", new DatePicker { SelectedDate = start.Date, Language = System.Windows.Markup.XmlLanguage.GetLanguage("vi-VN") });
            var gioBD = Nhap("Giờ bắt đầu (HH:mm) *", start.ToString("HH:mm"));
            var ngayKT = Truong("Ngày kết thúc *", new DatePicker { SelectedDate = end.Date, Language = System.Windows.Markup.XmlLanguage.GetLanguage("vi-VN") });
            var gioKT = Nhap("Giờ kết thúc (HH:mm) *", end.ToString("HH:mm"));
            var soCho = Nhap("Số chỗ tối đa *", (cu?.SoLuongToiDa ?? 50).ToString());
            var loai = Truong("Loại sự kiện *", new ComboBox { DisplayMemberPath = "Ten", SelectedValuePath = "Ma", ItemsSource = new[] {
                new { Ma = "HộiThảo", Ten = "Hội thảo" }, new { Ma = "VănHóaNghệThuật", Ten = "Văn hóa nghệ thuật" },
                new { Ma = "ThểThao", Ten = "Thể thao" }, new { Ma = "HọcThuật", Ten = "Học thuật" } } });
            loai.SelectedValue = cu?.LoaiSuKien ?? "HộiThảo";
            var moTa = Nhap("Mô tả", cu?.MoTa, true);
            NutLuu(() => {
                TimeSpan hourStart, hourEnd; int capacity;
                if (!ngayBD.SelectedDate.HasValue || !ngayKT.SelectedDate.HasValue ||
                    !TimeSpan.TryParseExact(gioBD.Text.Trim(), @"hh\:mm", CultureInfo.InvariantCulture, out hourStart) ||
                    !TimeSpan.TryParseExact(gioKT.Text.Trim(), @"hh\:mm", CultureInfo.InvariantCulture, out hourEnd) ||
                    hourStart.TotalHours >= 24 || hourEnd.TotalHours >= 24 ||
                    !int.TryParse(soCho.Text.Trim(), out capacity))
                    return (false, "Vui lòng nhập ngày, giờ (HH:mm) và số chỗ hợp lệ.");
                var sk = new SuKien { MaSuKien = cu?.MaSuKien, TenSuKien = ten.Text, MaCLB = (clb.SelectedItem as CauLacBo)?.MaCLB,
                    DiaDiem = diaDiem.Text, ThoiGianBatDau = ngayBD.SelectedDate.Value.Date.Add(hourStart),
                    ThoiGianKetThuc = ngayKT.SelectedDate.Value.Date.Add(hourEnd), SoLuongToiDa = capacity,
                    LoaiSuKien = loai.SelectedValue as string, MoTa = moTa.Text };
                var service = new DichVuSuKien();
                return cu == null ? service.ThemSuKien(sk) : service.CapNhatSuKien(sk);
            });
        }
    }

    public class TrangQuanLyTinTuc : UserControl
    {
        private readonly DichVuTinTuc _news = new DichVuTinTuc();
        private readonly DataGrid _bang;
        private readonly TextBlock _thongBao = new TextBlock { TextWrapping = TextWrapping.Wrap };
        private readonly Button _them, _sua, _an;
        public TrangQuanLyTinTuc()
        {
            Background = new SolidColorBrush(Color.FromRgb(240, 242, 245));
            var grid = GiaoDienNghiepVu.Trang("Quản lý tin tức");
            var toolbar = new WrapPanel();
            _them = GiaoDienNghiepVu.Nut("Đăng tin", (s,e) => MoForm(null));
            _sua = GiaoDienNghiepVu.Nut("Sửa tin", (s,e) => MoForm(_bang.SelectedItem as TinTuc));
            _an = GiaoDienNghiepVu.Nut("Ẩn tin", (s,e) => AnTin());
            toolbar.Children.Add(_them); toolbar.Children.Add(_sua); toolbar.Children.Add(_an);
            toolbar.Children.Add(GiaoDienNghiepVu.Nut("Làm mới", (s,e) => LamMoi()));
            GiaoDienNghiepVu.Them(grid, toolbar, 1);
            _bang = GiaoDienNghiepVu.Bang("Tiêu đề", "TieuDe", "Danh mục", "DanhMuc", "Tác giả", "TacGia", "Ngày đăng", "NgayDang", "Trạng thái", "TrangThai");
            _bang.SelectionChanged += (s,e) => {
                var tin = _bang.SelectedItem as TinTuc;
                _sua.IsEnabled = tin != null;
                _an.IsEnabled = tin != null && tin.TrangThai != "ĐãẨn";
                _thongBao.Text = tin?.NoiDung ?? "Chọn bài tin để sửa hoặc ẩn.";
            };
            GiaoDienNghiepVu.Them(grid, _bang, 2);
            GiaoDienNghiepVu.Them(grid, new ScrollViewer { Content = _thongBao, MaxHeight = 140, VerticalScrollBarVisibility = ScrollBarVisibility.Auto }, 3);
            Content = grid; LamMoi();
        }
        public void LamMoi()
        {
            _bang.ItemsSource = _news.LayTinDuocQuanLy();
            _them.IsEnabled = DichVuPhanQuyen.LaAdmin || new DichVuCLB().LayCLBDuocQuanLy().Count > 0;
            _sua.IsEnabled = _an.IsEnabled = false;
        }
        private void MoForm(TinTuc tin)
        {
            if (tin == null && !_them.IsEnabled) return;
            if (new FormTinTuc(tin) { Owner = Window.GetWindow(this) }.ShowDialog() == true) LamMoi();
        }
        private void AnTin()
        {
            var tin = _bang.SelectedItem as TinTuc;
            if (tin == null || !GiaoDienNghiepVu.XacNhan(this, "Ẩn bài tin '" + tin.TieuDe + "'?")) return;
            var result = _news.AnTin(tin.MaTinTuc);
            LamMoi(); _thongBao.Text = result.ThongBao;
        }
    }

    internal class FormTinTuc : FormNghiepVu
    {
        public FormTinTuc(TinTuc cu) : base(cu == null ? "Đăng tin" : "Sửa tin")
        {
            var ten = Nhap("Tiêu đề *", cu?.TieuDe);
            var clubs = new DichVuCLB().LayCLBDuocQuanLy();
            if (DichVuPhanQuyen.LaAdmin) clubs.Insert(0, new CauLacBo { MaCLB = null, TenCLB = "Nhà trường" });
            var clb = Truong("Đơn vị đăng tin *", new ComboBox { ItemsSource = clubs, DisplayMemberPath = "TenCLB" });
            clb.SelectedItem = clubs.FirstOrDefault(c => c.MaCLB == cu?.MaCLB) ?? clubs.FirstOrDefault();
            var danhMuc = Truong("Danh mục *", new ComboBox { DisplayMemberPath = "Ten", SelectedValuePath = "Ma", ItemsSource = new[] {
                new { Ma = "ThôngBáo", Ten = "Thông báo" }, new { Ma = "SựKiện", Ten = "Sự kiện" },
                new { Ma = "HoạtĐộngSinhViên", Ten = "Hoạt động sinh viên" }, new { Ma = "CLB", Ten = "CLB" } } });
            danhMuc.SelectedValue = cu?.DanhMuc ?? "ThôngBáo";
            var tomTat = Nhap("Tóm tắt", cu?.TomTat, true);
            var noiDung = Nhap("Nội dung *", cu?.NoiDung, true);
            noiDung.MinHeight = 180;
            NutLuu(() => {
                if (clb.SelectedItem == null) return (false, "Vui lòng chọn đơn vị đăng tin.");
                return new DichVuTinTuc().LuuTin(new TinTuc { MaTinTuc = cu?.MaTinTuc, TieuDe = ten.Text,
                    MaCLB = (clb.SelectedItem as CauLacBo).MaCLB, DanhMuc = danhMuc.SelectedValue as string,
                    TomTat = tomTat.Text, NoiDung = noiDung.Text }, cu == null);
            });
        }
    }

    internal class NhanTrangThai : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value as string)
            {
                case "ChờDuyệt": return "Chờ duyệt";
                case "ĐãDuyệt": return "Đã duyệt";
                case "TừChối": return "Từ chối";
                case "SắpDiễnRa": return "Sắp diễn ra";
                case "ĐangDiễnRa": return "Đang diễn ra";
                case "ĐãKếtThúc": return "Đã kết thúc";
                case "ĐãHủy": return "Đã hủy";
                case "ĐãẨn": return "Đã ẩn";
                case "ThôngBáo": return "Thông báo";
                case "SựKiện": return "Sự kiện";
                case "HoạtĐộngSinhViên": return "Hoạt động sinh viên";
                default: return value;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
