using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CLB_EVENT.Models;
using CLB_EVENT.Services;

namespace CLB_EVENT
{
    /// <summary>
    /// Trang đăng ký sự kiện: xem danh sách và đăng ký tham gia sự kiện
    /// </summary>
    public partial class TrangDangKySuKien : UserControl
    {
        // Các dịch vụ cần dùng
        private readonly DichVuSuKien _dichVuSuKien;
        private readonly DichVuCLB _dichVuCLB;

        // Sự kiện được chọn để đăng ký
        private SuKien _suKienDangChon;
        private bool _dangTai;

        public TrangDangKySuKien()
        {
            InitializeComponent();
            _dichVuSuKien = new DichVuSuKien();
            _dichVuCLB = new DichVuCLB();

            TaiDuLieuBanDau();
        }

        public void LamMoi()
        {
            var filter = cbLocCLB.SelectedItem as string;
            _dangTai = true;
            var choices = _dichVuCLB.LayDanhSachCLB().Select(c => c.TenCLB).ToList();
            choices.Insert(0, "Tất cả CLB");
            cbLocCLB.ItemsSource = choices;
            cbLocCLB.SelectedItem = choices.Contains(filter) ? filter : choices[0];
            _dangTai = false;
            TaiDanhSachSuKien(tbTimKiem.Text?.Trim(), cbLocCLB.SelectedItem as string);
            TaiSuKienDaDangKy();
            if (_suKienDangChon != null)
            {
                var sk = _dichVuSuKien.LaySuKienTheoMa(_suKienDangChon.MaSuKien);
                if (sk != null && sk.TrangThai == "SắpDiễnRa" && _dichVuCLB.TimCLBTheoMa(sk.MaCLB)?.TrangThaiHoatDong == true)
                {
                    tbTenSuKien.Text = sk.TenSuKien;
                    tbThoiGian.Text = $"{sk.ThoiGianBatDau:dd/MM/yyyy HH:mm} – {sk.ThoiGianKetThuc:dd/MM/yyyy HH:mm}";
                    tbDiaDiem.Text = sk.DiaDiem;
                    tbTenCLB.Text = _dichVuCLB.TimCLBTheoMa(sk.MaCLB)?.TenCLB;
                    CapNhatNutDangKy();
                }
                else
                {
                    _suKienDangChon = null;
                    panelThongTinDangKy.Visibility = Visibility.Collapsed;
                    borderChuaChon.Visibility = Visibility.Visible;
                }
            }
        }

        private void CapNhatNutDangKy()
        {
            var user = DichVuNguoiDung.NguoiDungHienTai;
            bool registered = user != null && _suKienDangChon != null &&
                _dichVuSuKien.LayDangKyTheoNguoiDung(user.MaNguoiDung).Any(d => d.MaSuKien == _suKienDangChon.MaSuKien);
            nutDangKySuKien.IsEnabled = user != null && _suKienDangChon?.ConChoTrong == true && !registered;
            nutDangKySuKien.Content = registered ? "ĐÃ ĐĂNG KÝ" : _suKienDangChon?.ConChoTrong != true ? "ĐÃ HẾT CHỖ" : "ĐĂNG KÝ THAM GIA";
        }

        /// <summary>
        /// Tải dữ liệu ban đầu: danh sách CLB lọc và sự kiện sắp tới
        /// </summary>
        private void TaiDuLieuBanDau()
        {
            // Tải danh sách CLB vào ComboBox lọc
            var danhSachCLB = _dichVuCLB.LayDanhSachCLB()
                .Select(c => c.TenCLB)
                .ToList();
            danhSachCLB.Insert(0, "Tất cả CLB");
            cbLocCLB.ItemsSource = danhSachCLB;
            cbLocCLB.SelectedIndex = 0;

            // Tải danh sách sự kiện
            TaiDanhSachSuKien(string.Empty, string.Empty);

            // Tải sự kiện người dùng đã đăng ký
            TaiSuKienDaDangKy();
        }

        /// <summary>
        /// Tải và hiển thị danh sách sự kiện theo bộ lọc
        /// </summary>
        private void TaiDanhSachSuKien(string tuKhoa, string tenCLBLoc)
        {
            panelDanhSachSuKien.Children.Clear();

            // Lấy danh sách sự kiện sắp tới
            var danhSach = _dichVuSuKien.LayDanhSachSuKienSapToi();

            // Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                danhSach = danhSach
                    .Where(sk => sk.TenSuKien.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            // Lọc theo CLB
            if (!string.IsNullOrWhiteSpace(tenCLBLoc) && tenCLBLoc != "Tất cả CLB")
            {
                danhSach = danhSach
                    .Where(sk => sk.TenCLB == tenCLBLoc)
                    .ToList();
            }

            // Cập nhật số lượng sự kiện
            tbSoSuKien.Text = danhSach.Count + " sự kiện";

            if (danhSach.Count == 0)
            {
                // Hiển thị thông báo trống
                var tbTrong = new TextBlock
                {
                    Text = "Không có sự kiện nào phù hợp",
                    FontSize = 13,
                    Foreground = new SolidColorBrush(Color.FromRgb(158, 158, 158)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 40, 0, 0)
                };
                panelDanhSachSuKien.Children.Add(tbTrong);
                return;
            }

            // Tạo thẻ hiển thị cho từng sự kiện
            foreach (var suKien in danhSach)
            {
                var theCard = TaoTheHienThiSuKien(suKien);
                panelDanhSachSuKien.Children.Add(theCard);
            }
        }

        /// <summary>
        /// Tạo một thẻ (card) hiển thị thông tin sự kiện
        /// </summary>
        private Border TaoTheHienThiSuKien(SuKien suKien)
        {
            // Tạo border chứa thẻ sự kiện
            var vienNgoai = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(0, 0, 0, 8),
                Padding = new Thickness(18, 14, 18, 14),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            // Hiệu ứng hover
            vienNgoai.MouseEnter += (s, e) => vienNgoai.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            vienNgoai.MouseLeave += (s, e) => vienNgoai.Background = Brushes.White;

            // Khi click vào thẻ, chọn sự kiện này để đăng ký
            var suKienNay = suKien;
            vienNgoai.MouseLeftButtonDown += (s, e) => ChonSuKien(suKienNay);

            // Layout nội dung thẻ
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

            // Hiển thị ngày tháng (cột trái)
            var vienNgay = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(227, 242, 253)),
                CornerRadius = new CornerRadius(8),
                Width = 52,
                Height = 55,
                VerticalAlignment = VerticalAlignment.Top
            };
            var stackNgay = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            stackNgay.Children.Add(new TextBlock
            {
                Text = suKien.ThoiGianBatDau.Day.ToString(),
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(21, 101, 192)),
                HorizontalAlignment = HorizontalAlignment.Center
            });
            stackNgay.Children.Add(new TextBlock
            {
                Text = "T" + suKien.ThoiGianBatDau.Month,
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(21, 101, 192)),
                HorizontalAlignment = HorizontalAlignment.Center
            });
            vienNgay.Child = stackNgay;
            Grid.SetColumn(vienNgay, 0);
            grid.Children.Add(vienNgay);

            // Thông tin sự kiện (cột giữa)
            var stackThongTin = new StackPanel { Margin = new Thickness(14, 0, 0, 0), VerticalAlignment = VerticalAlignment.Top };
            stackThongTin.Children.Add(new TextBlock
            {
                Text = suKien.TenSuKien,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(33, 33, 33)),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 5)
            });
            stackThongTin.Children.Add(new TextBlock
            {
                Text = "🎯 " + suKien.TenCLB,
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(21, 101, 192)),
                Margin = new Thickness(0, 0, 0, 3)
            });
            stackThongTin.Children.Add(new TextBlock
            {
                Text = "📍 " + suKien.DiaDiem,
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(117, 117, 117)),
                Margin = new Thickness(0, 0, 0, 3)
            });
            stackThongTin.Children.Add(new TextBlock
            {
                Text = string.Format("⏰ {0:HH:mm} - {1:HH:mm}", suKien.ThoiGianBatDau, suKien.ThoiGianKetThuc),
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(117, 117, 117))
            });
            Grid.SetColumn(stackThongTin, 1);
            grid.Children.Add(stackThongTin);

            // Nhãn "Chỗ trống" (cột phải)
            var vienConCho = new Border
            {
                Background = suKien.SoChoConLai > 0
                    ? new SolidColorBrush(Color.FromRgb(232, 245, 233))
                    : new SolidColorBrush(Color.FromRgb(255, 235, 238)),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(8, 5, 8, 5),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            vienConCho.Child = new TextBlock
            {
                Text = suKien.SoChoConLai > 0
                    ? suKien.SoChoConLai + " chỗ\ntrống"
                    : "Hết chỗ",
                FontSize = 11,
                TextAlignment = TextAlignment.Center,
                Foreground = suKien.SoChoConLai > 0
                    ? new SolidColorBrush(Color.FromRgb(46, 125, 50))
                    : new SolidColorBrush(Color.FromRgb(198, 40, 40))
            };
            Grid.SetColumn(vienConCho, 2);
            grid.Children.Add(vienConCho);

            vienNgoai.Child = grid;
            return vienNgoai;
        }

        /// <summary>
        /// Xử lý khi người dùng chọn một sự kiện để đăng ký
        /// </summary>
        private void ChonSuKien(SuKien suKien)
        {
            _suKienDangChon = suKien;

            // Hiển thị thông tin sự kiện trên form đăng ký
            tbTenSuKien.Text = suKien.TenSuKien;
            tbThoiGian.Text = string.Format("{0:dd/MM/yyyy HH:mm} - {1:HH:mm}",
                suKien.ThoiGianBatDau, suKien.ThoiGianKetThuc);
            tbDiaDiem.Text = suKien.DiaDiem;
            tbTenCLB.Text = suKien.TenCLB;

            // Ẩn thông báo "chưa chọn", hiện form đăng ký
            borderChuaChon.Visibility = Visibility.Collapsed;
            panelThongTinDangKy.Visibility = Visibility.Visible;
            borderThongBaoDangKy.Visibility = Visibility.Collapsed;
            tbGhiChu.Clear();
            CapNhatNutDangKy();
        }

        /// <summary>
        /// Xử lý khi nhấn nút Đăng Ký sự kiện
        /// </summary>
        private void NutDangKySuKien_Click(object sender, RoutedEventArgs e)
        {
            if (_suKienDangChon == null) return;

            var nguoiDung = DichVuNguoiDung.NguoiDungHienTai;
            if (nguoiDung == null) return;

            // Kiểm tra còn chỗ trống không
            if (_suKienDangChon.SoChoConLai <= 0)
            {
                HienThiKetQua("❌ Sự kiện đã hết chỗ, không thể đăng ký!", false);
                return;
            }

            // Thực hiện đăng ký sự kiện
            var ketQua = _dichVuSuKien.DangKySuKien(
                nguoiDung.MaNguoiDung,
                _suKienDangChon.MaSuKien,
                tbGhiChu.Text?.Trim() ?? string.Empty);

            if (ketQua.ThanhCong)
            {
                HienThiKetQua("✅ Đăng ký thành công! Chúc bạn tham gia vui vẻ.", true);
                // Cập nhật lại danh sách sự kiện và đã đăng ký
                LamMoi();
            }
            else
            {
                HienThiKetQua("❌ " + ketQua.ThongBao, false);
            }
        }

        /// <summary>
        /// Hiển thị thông báo kết quả đăng ký
        /// </summary>
        private void HienThiKetQua(string thongBao, bool thanhCong)
        {
            tbThongBaoDangKy.Text = thongBao;
            borderThongBaoDangKy.Background = thanhCong
                ? new SolidColorBrush(Color.FromRgb(232, 245, 233))
                : new SolidColorBrush(Color.FromRgb(255, 235, 238));
            tbThongBaoDangKy.Foreground = thanhCong
                ? new SolidColorBrush(Color.FromRgb(27, 94, 32))
                : new SolidColorBrush(Color.FromRgb(198, 40, 40));
            borderThongBaoDangKy.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Tải danh sách sự kiện người dùng đã đăng ký
        /// </summary>
        private void TaiSuKienDaDangKy()
        {
            var nguoiDung = DichVuNguoiDung.NguoiDungHienTai;
            if (nguoiDung == null) return;

            var danhSachDaDangKy = _dichVuSuKien.LayDangKyTheoNguoiDung(nguoiDung.MaNguoiDung)
                .Select(dk => new
                {
                    MaDangKy = dk.MaDangKy,
                    TenSuKien = _dichVuSuKien.LaySuKienTheoMa(dk.MaSuKien)?.TenSuKien ?? "Không xác định",
                    TrangThai = dk.TrangThai == "SựKiệnĐãHủy" ? "Sự kiện đã bị hủy" : "✅ Đã đăng ký",
                    CoTheHuy = dk.TrangThai == "ĐãĐăngKý" && _dichVuSuKien.LaySuKienTheoMa(dk.MaSuKien)?.TrangThai == "SắpDiễnRa",
                    NgayDangKy = "📅 " + dk.NgayDangKy.ToString("dd/MM/yyyy")
                })
                .ToList();

            if (danhSachDaDangKy.Count > 0)
            {
                borderTrongDangKy.Visibility = Visibility.Collapsed;
                dsSuKienDaDangKy.ItemsSource = danhSachDaDangKy;
            }
            else
            {
                borderTrongDangKy.Visibility = Visibility.Visible;
                dsSuKienDaDangKy.ItemsSource = null;
            }
        }

        /// <summary>
        /// Xử lý nút tìm kiếm sự kiện
        /// </summary>
        private void NutTimKiem_Click(object sender, RoutedEventArgs e)
        {
            string tuKhoa = tbTimKiem.Text?.Trim() ?? string.Empty;
            string clbLoc = cbLocCLB.SelectedItem?.ToString() ?? string.Empty;
            TaiDanhSachSuKien(tuKhoa, clbLoc);
        }

        /// <summary>
        /// Xử lý khi thay đổi bộ lọc CLB
        /// </summary>
        private void CbLocCLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dangTai || _dichVuSuKien == null) return;
            string tuKhoa = tbTimKiem.Text?.Trim() ?? string.Empty;
            string clbLoc = cbLocCLB.SelectedItem?.ToString() ?? string.Empty;
            TaiDanhSachSuKien(tuKhoa, clbLoc);
        }

        private void NutHuyDangKy_Click(object sender, RoutedEventArgs e)
        {
            var user = DichVuNguoiDung.NguoiDungHienTai;
            if (user == null) return;
            if (!GiaoDienNghiepVu.XacNhan(this, "Hủy đăng ký tham gia sự kiện này?")) return;
            var result = _dichVuSuKien.HuyDangKy((sender as Button)?.Tag as string, user.MaNguoiDung);
            LamMoi();
            MessageBox.Show(Window.GetWindow(this), result.ThongBao, "Đăng ký sự kiện", MessageBoxButton.OK,
                result.ThanhCong ? MessageBoxImage.Information : MessageBoxImage.Warning);
        }
    }
}
