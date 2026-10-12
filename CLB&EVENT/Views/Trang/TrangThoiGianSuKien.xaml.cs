using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CLB_EVENT.Models;
using CLB_EVENT.Services;

namespace CLB_EVENT
{
    /// <summary>
    /// Trang thời gian sự kiện: hiển thị lịch tháng và danh sách sự kiện
    /// </summary>
    public partial class TrangThoiGianSuKien : UserControl
    {
        // Dịch vụ sự kiện
        private readonly DichVuSuKien _dichVuSuKien;

        // Tháng và năm hiện đang xem
        private int _thangHienTai;
        private int _namHienTai;

        // Ngày được chọn trên lịch
        private DateTime? _ngayDangChon;

        public TrangThoiGianSuKien()
        {
            InitializeComponent();
            _dichVuSuKien = new DichVuSuKien();

            // Khởi tạo với tháng hiện tại
            _thangHienTai = DateTime.Now.Month;
            _namHienTai = DateTime.Now.Year;

            // Vẽ lịch và tải sự kiện
            VeLichThang(_thangHienTai, _namHienTai);
            TaiSuKienSapToi();
        }

        public void LamMoi()
        {
            VeLichThang(_thangHienTai, _namHienTai);
            if (_ngayDangChon.HasValue) TaiSuKienTheoNgay(_ngayDangChon.Value);
            else TaiSuKienSapToi();
        }

        /// <summary>
        /// Vẽ lưới lịch tháng theo tháng và năm được chỉ định
        /// </summary>
        private void VeLichThang(int thang, int nam)
        {
            // Cập nhật tiêu đề tháng năm
            tbThangNam.Text = string.Format("Tháng {0} / {1}", thang, nam);

            // Xóa lưới cũ
            luoiLich.Children.Clear();
            luoiLich.RowDefinitions.Clear();
            luoiLich.ColumnDefinitions.Clear();

            // Tạo 7 cột (7 ngày trong tuần)
            for (int cot = 0; cot < 7; cot++)
                luoiLich.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Tính số hàng cần thiết (tối đa 6 tuần)
            var ngayDauThang = new DateTime(nam, thang, 1);
            int ngayBatDauTuan = (int)ngayDauThang.DayOfWeek; // Chủ nhật = 0
            int soNgayTrongThang = DateTime.DaysInMonth(nam, thang);
            int soHang = (int)Math.Ceiling((ngayBatDauTuan + soNgayTrongThang) / 7.0);

            for (int hang = 0; hang < soHang; hang++)
                luoiLich.RowDefinitions.Add(new RowDefinition { Height = new GridLength(36) });

            // Lấy danh sách ngày có sự kiện
            var ngayCoSuKien = LayNgayCoSuKien(thang, nam);

            // Điền số ngày vào lưới
            int ngayHienTai = 1;
            for (int hang = 0; hang < soHang && ngayHienTai <= soNgayTrongThang; hang++)
            {
                for (int cot = 0; cot < 7; cot++)
                {
                    // Bỏ qua các ô trước ngày đầu tháng
                    if (hang == 0 && cot < ngayBatDauTuan) continue;
                    if (ngayHienTai > soNgayTrongThang) break;

                    int ngay = ngayHienTai;
                    bool laHomNay = (ngay == DateTime.Now.Day && thang == DateTime.Now.Month && nam == DateTime.Now.Year);
                    bool coSuKien = ngayCoSuKien.Contains(ngay);
                    bool laChuNhat = (cot == 0);

                    // Tạo ô ngày
                    var vienNgay = TaoONgay(ngay, laHomNay, coSuKien, laChuNhat, thang, nam);
                    Grid.SetRow(vienNgay, hang);
                    Grid.SetColumn(vienNgay, cot);
                    luoiLich.Children.Add(vienNgay);

                    ngayHienTai++;
                }
            }

            // Cập nhật thống kê
            CapNhatThongKe(thang, nam);
        }

        /// <summary>
        /// Tạo ô hiển thị một ngày trong lịch
        /// </summary>
        private Border TaoONgay(int ngay, bool laHomNay, bool coSuKien, bool laChuNhat, int thang, int nam)
        {
            // Màu nền: hôm nay màu xanh, ngày có sự kiện màu vàng nhạt, còn lại trong suốt
            Brush maNen;
            if (laHomNay)
                maNen = new SolidColorBrush(Color.FromRgb(21, 101, 192));
            else if (coSuKien)
                maNen = new SolidColorBrush(Color.FromRgb(255, 243, 224));
            else
                maNen = Brushes.Transparent;

            // Màu chữ
            Brush maChu;
            if (laHomNay)
                maChu = Brushes.White;
            else if (laChuNhat)
                maChu = new SolidColorBrush(Color.FromRgb(229, 57, 53));
            else
                maChu = new SolidColorBrush(Color.FromRgb(66, 66, 66));

            var vienNgay = new Border
            {
                Background = maNen,
                CornerRadius = new CornerRadius(laHomNay ? 18 : 4),
                Width = 32,
                Height = 32,
                Margin = new Thickness(1),
                Cursor = System.Windows.Input.Cursors.Hand,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var tbNgay = new TextBlock
            {
                Text = ngay.ToString(),
                FontSize = 12,
                Foreground = maChu,
                FontWeight = laHomNay ? FontWeights.Bold : FontWeights.Normal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            vienNgay.Child = tbNgay;

            // Hiệu ứng hover và click
            int ngayNay = ngay;
            vienNgay.MouseEnter += (s, e) =>
            {
                if (!laHomNay)
                    vienNgay.Background = new SolidColorBrush(Color.FromRgb(227, 242, 253));
            };
            vienNgay.MouseLeave += (s, e) =>
            {
                vienNgay.Background = maNen;
            };
            vienNgay.MouseLeftButtonDown += (s, e) =>
            {
                _ngayDangChon = new DateTime(nam, thang, ngayNay);
                TaiSuKienTheoNgay(_ngayDangChon.Value);
            };

            // Dấu chấm nhỏ nếu có sự kiện
            if (coSuKien && !laHomNay)
            {
                var chungCha = new Grid();
                chungCha.Children.Add(vienNgay);
                var cham = new Border
                {
                    Width = 5, Height = 5,
                    CornerRadius = new CornerRadius(3),
                    Background = new SolidColorBrush(Color.FromRgb(245, 124, 0)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 0, 1)
                };
                chungCha.Children.Add(cham);
                var viTri = new Border
                {
                    Width = 36, Height = 36,
                    Margin = new Thickness(1),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Child = chungCha
                };
                return viTri;
            }

            return vienNgay;
        }

        /// <summary>
        /// Lấy danh sách các ngày trong tháng có sự kiện diễn ra
        /// </summary>
        private HashSet<int> LayNgayCoSuKien(int thang, int nam)
        {
            var tatCaSuKien = _dichVuSuKien.LaySuKienTheoThang(thang, nam);
            var ketQua = new HashSet<int>();

            foreach (var sk in tatCaSuKien)
            {
                var first = new DateTime(nam, thang, 1);
                var last = first.AddMonths(1).AddDays(-1);
                var start = sk.ThoiGianBatDau.Date < first ? first : sk.ThoiGianBatDau.Date;
                var end = sk.ThoiGianKetThuc.AddTicks(-1).Date > last ? last : sk.ThoiGianKetThuc.AddTicks(-1).Date;
                for (var day = start; day <= end; day = day.AddDays(1)) ketQua.Add(day.Day);
            }

            return ketQua;
        }

        /// <summary>
        /// Cập nhật thống kê số sự kiện trong tháng
        /// </summary>
        private void CapNhatThongKe(int thang, int nam)
        {
            var tatCaSuKien = _dichVuSuKien.LaySuKienTheoThang(thang, nam);
            int tongSuKienThang = tatCaSuKien.Count;
            int suKienConLai = tatCaSuKien.Count(sk => sk.ThoiGianKetThuc > DateTime.Now);

            tbSoSuKienThang.Text = tongSuKienThang.ToString();
            tbSoSuKienConLai.Text = suKienConLai.ToString();
        }

        /// <summary>
        /// Tải và hiển thị tất cả sự kiện sắp tới
        /// </summary>
        private void TaiSuKienSapToi()
        {
            tbTieuDeDanhSach.Text = "TẤT CẢ SỰ KIỆN SẮP TỚI";
            var danhSach = _dichVuSuKien.LayDanhSachSuKienSapToi();
            HienThiDanhSachSuKien(danhSach);
        }

        /// <summary>
        /// Tải và hiển thị sự kiện theo ngày được chọn
        /// </summary>
        private void TaiSuKienTheoNgay(DateTime ngay)
        {
            tbTieuDeDanhSach.Text = string.Format("SỰ KIỆN NGÀY {0:dd/MM/yyyy}", ngay);

            var danhSach = _dichVuSuKien.LayDanhSachSuKien()
                .Where(sk => sk.TrangThai != "ĐãHủy" && sk.ThoiGianBatDau < ngay.Date.AddDays(1) && sk.ThoiGianKetThuc > ngay.Date)
                .ToList();

            HienThiDanhSachSuKien(danhSach);
        }

        /// <summary>
        /// Hiển thị danh sách sự kiện vào panel
        /// </summary>
        private void HienThiDanhSachSuKien(List<SuKien> danhSach)
        {
            panelSuKien.Children.Clear();

            if (danhSach == null || danhSach.Count == 0)
            {
                var theTrong = new Border { Padding = new Thickness(20, 40, 20, 40) };
                var stackTrong = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
                stackTrong.Children.Add(new TextBlock
                {
                    Text = "📭",
                    FontSize = 36,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                stackTrong.Children.Add(new TextBlock
                {
                    Text = "Không có sự kiện nào",
                    FontSize = 13,
                    Foreground = new SolidColorBrush(Color.FromRgb(158, 158, 158)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 8, 0, 0)
                });
                theTrong.Child = stackTrong;
                panelSuKien.Children.Add(theTrong);
                return;
            }

            // Tạo thẻ cho từng sự kiện
            foreach (var suKien in danhSach)
            {
                var the = new Border
                {
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(8),
                    Margin = new Thickness(0, 0, 0, 8),
                    Padding = new Thickness(18, 14, 18, 14),
                    Cursor = System.Windows.Input.Cursors.Hand
                };
                the.MouseEnter += (s, e) => the.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                the.MouseLeave += (s, e) => the.Background = Brushes.White;

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Đường màu bên trái (màu theo CLB)
                var thanMau = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(21, 101, 192)),
                    CornerRadius = new CornerRadius(3),
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                Grid.SetColumn(thanMau, 0);
                grid.Children.Add(thanMau);

                // Thông tin sự kiện
                var stackInfo = new StackPanel { Margin = new Thickness(14, 0, 0, 0) };
                stackInfo.Children.Add(new TextBlock
                {
                    Text = suKien.TenSuKien,
                    FontSize = 14,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Color.FromRgb(33, 33, 33)),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 5)
                });

                var stackChiTiet = new StackPanel { Orientation = Orientation.Horizontal };
                stackChiTiet.Children.Add(new TextBlock
                {
                    Text = string.Format("⏰ {0:dd/MM/yyyy HH:mm}", suKien.ThoiGianBatDau),
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Color.FromRgb(117, 117, 117)),
                    Margin = new Thickness(0, 0, 15, 0)
                });
                stackChiTiet.Children.Add(new TextBlock
                {
                    Text = "📍 " + suKien.DiaDiem,
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Color.FromRgb(117, 117, 117))
                });
                stackInfo.Children.Add(stackChiTiet);

                stackInfo.Children.Add(new TextBlock
                {
                    Text = "🎯 " + suKien.TenCLB,
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Color.FromRgb(21, 101, 192)),
                    Margin = new Thickness(0, 4, 0, 0)
                });

                Grid.SetColumn(stackInfo, 1);
                grid.Children.Add(stackInfo);

                the.Child = grid;
                panelSuKien.Children.Add(the);
            }
        }

        /// <summary>
        /// Chuyển về tháng trước
        /// </summary>
        private void NutThangTruoc_Click(object sender, RoutedEventArgs e)
        {
            _thangHienTai--;
            if (_thangHienTai < 1)
            {
                _thangHienTai = 12;
                _namHienTai--;
            }
            VeLichThang(_thangHienTai, _namHienTai);
            TaiSuKienThang();
        }

        /// <summary>
        /// Chuyển đến tháng sau
        /// </summary>
        private void NutThangSau_Click(object sender, RoutedEventArgs e)
        {
            _thangHienTai++;
            if (_thangHienTai > 12)
            {
                _thangHienTai = 1;
                _namHienTai++;
            }
            VeLichThang(_thangHienTai, _namHienTai);
            TaiSuKienThang();
        }

        private void TaiSuKienThang()
        {
            _ngayDangChon = null;
            tbTieuDeDanhSach.Text = $"SỰ KIỆN THÁNG {_thangHienTai}/{_namHienTai}";
            HienThiDanhSachSuKien(_dichVuSuKien.LaySuKienTheoThang(_thangHienTai, _namHienTai));
        }

        /// <summary>
        /// Xem tất cả sự kiện sắp tới
        /// </summary>
        private void NutXemTatCa_Click(object sender, RoutedEventArgs e)
        {
            _ngayDangChon = null;
            TaiSuKienSapToi();
        }
    }
}
