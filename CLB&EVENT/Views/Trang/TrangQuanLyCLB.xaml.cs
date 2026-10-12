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
    /// Trang quản lý CLB: thêm, sửa, xóa và xem chi tiết các câu lạc bộ
    /// Dành cho Admin và Quản lý CLB
    /// </summary>
    public partial class TrangQuanLyCLB : UserControl
    {
        // Dịch vụ quản lý CLB
        private readonly DichVuCLB _dichVuCLB;

        // CLB đang được chọn để xem chi tiết hoặc chỉnh sửa
        private CauLacBo _clbDangChon;

        // Chế độ form: "Them" hoặc "Sua"
        private string _cheDoDForm;

        public TrangQuanLyCLB()
        {
            InitializeComponent();
            _dichVuCLB = new DichVuCLB();

            // Kiểm tra vai trò người dùng
            KiemTraVaiTro();

            // Tải danh sách CLB
            TaiDanhSachCLB(string.Empty);
        }

        public void LamMoi()
        {
            KiemTraVaiTro();
            TaiDanhSachCLB(tbTimKiem.Text?.Trim() ?? string.Empty);
            if (_clbDangChon != null && _clbDangChon.TrangThaiHoatDong) ChonCLB(_clbDangChon);
        }

        private void TaiQuanLy(string ma)
        {
            var choices = new DichVuNguoiDung().LayDanhSachNguoiDung()
                .Where(n => n.VaiTro == "QuanLyCLB" && n.TrangThaiHoatDong).ToList();
            choices.Insert(0, new NguoiDung { MaNguoiDung = null, HoTen = "Chưa phân công" });
            cbQuanLyForm.ItemsSource = choices;
            cbQuanLyForm.SelectedValue = ma;
            cbQuanLyForm.IsEnabled = DichVuPhanQuyen.LaAdmin;
        }

        /// <summary>
        /// Kiểm tra vai trò: chỉ Admin mới được thêm/sửa/xóa CLB
        /// </summary>
        private void KiemTraVaiTro()
        {
            var nguoiDung = DichVuNguoiDung.NguoiDungHienTai;
            bool laAdmin = nguoiDung != null && nguoiDung.VaiTro == "QuanLyAdmin";

            // Nút thêm CLB chỉ hiện với Admin
            nutThemCLB.Visibility = laAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Tải và hiển thị danh sách CLB theo từ khóa tìm kiếm
        /// </summary>
        private void TaiDanhSachCLB(string tuKhoa)
        {
            panelDanhSachCLB.Children.Clear();

            var danhSach = _dichVuCLB.LayCLBDuocQuanLy();

            // Lọc theo từ khóa nếu có
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                danhSach = danhSach
                    .Where(c => c.TenCLB.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            // Cập nhật số lượng
            tbSoCLB.Text = danhSach.Count + " CLB";

            if (danhSach.Count == 0)
            {
                var tbTrong = new TextBlock
                {
                    Text = "Không tìm thấy CLB nào",
                    FontSize = 13,
                    Foreground = new SolidColorBrush(Color.FromRgb(158, 158, 158)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 40, 0, 0)
                };
                panelDanhSachCLB.Children.Add(tbTrong);
                return;
            }

            // Tạo thẻ cho từng CLB
            foreach (var clb in danhSach)
            {
                var the = TaoTheCLB(clb);
                panelDanhSachCLB.Children.Add(the);
            }
        }

        /// <summary>
        /// Tạo thẻ hiển thị thông tin tóm tắt của một CLB
        /// </summary>
        private Border TaoTheCLB(CauLacBo clb)
        {
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

            var clbNay = clb;
            vienNgoai.MouseLeftButtonDown += (s, e) => ChonCLB(clbNay);

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Avatar CLB (dùng chữ cái đầu)
            var vienAvatar = new Border
            {
                Width = 44,
                Height = 44,
                CornerRadius = new CornerRadius(22),
                Background = new SolidColorBrush(Color.FromRgb(21, 101, 192)),
                VerticalAlignment = VerticalAlignment.Top
            };
            vienAvatar.Child = new TextBlock
            {
                Text = clb.TenCLB.Length > 0 ? clb.TenCLB[0].ToString().ToUpper() : "C",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(vienAvatar, 0);
            grid.Children.Add(vienAvatar);

            // Thông tin CLB
            var stackInfo = new StackPanel { Margin = new Thickness(12, 0, 0, 0) };
            stackInfo.Children.Add(new TextBlock
            {
                Text = clb.TenCLB,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(33, 33, 33))
            });
            stackInfo.Children.Add(new TextBlock
            {
                Text = "👨‍💼 " + clb.ChuNhiem,
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(117, 117, 117)),
                Margin = new Thickness(0, 4, 0, 0)
            });
            stackInfo.Children.Add(new TextBlock
            {
                Text = string.Format("👥 {0} thành viên", clb.SoThanhVien),
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(117, 117, 117))
            });
            Grid.SetColumn(stackInfo, 1);
            grid.Children.Add(stackInfo);

            vienNgoai.Child = grid;
            return vienNgoai;
        }

        /// <summary>
        /// Xử lý khi chọn một CLB từ danh sách
        /// </summary>
        private void ChonCLB(CauLacBo clb)
        {
            _clbDangChon = clb;

            // Hiện panel chi tiết
            borderChiTietCLB.Visibility = Visibility.Visible;
            borderFormCLB.Visibility = Visibility.Collapsed;
            borderHuongDan.Visibility = Visibility.Collapsed;

            // Điền thông tin
            tbTenCLBChiTiet.Text = clb.TenCLB;
            tbMoTaCLB.Text = clb.MoTa;
            tbChuNhiem.Text = clb.ChuNhiem;
            tbNgayThanhLap.Text = clb.NgayThanhLap.ToString("dd/MM/yyyy");
            tbSoThanhVien.Text = clb.SoThanhVien.ToString() + " người";

            // Hiện nút chỉnh sửa/xóa nếu là Admin
            var nguoiDung = DichVuNguoiDung.NguoiDungHienTai;
            bool laAdmin = nguoiDung != null && nguoiDung.VaiTro == "QuanLyAdmin";
            panelNutQuanLy.Visibility = DichVuPhanQuyen.QuanLyCLB(clb) ? Visibility.Visible : Visibility.Collapsed;
            nutXoaCLB.Visibility = laAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Xử lý tìm kiếm theo text thay đổi
        /// </summary>
        private void TbTimKiem_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_dichVuCLB == null || panelDanhSachCLB == null) return;
            TaiDanhSachCLB(tbTimKiem.Text?.Trim() ?? string.Empty);
        }

        /// <summary>
        /// Xử lý nút tìm kiếm
        /// </summary>
        private void NutTimKiem_Click(object sender, RoutedEventArgs e)
        {
            TaiDanhSachCLB(tbTimKiem.Text?.Trim() ?? string.Empty);
        }

        /// <summary>
        /// Mở form thêm CLB mới
        /// </summary>
        private void NutThemCLB_Click(object sender, RoutedEventArgs e)
        {
            if (!DichVuPhanQuyen.LaAdmin) return;
            _cheDoDForm = "Them";
            _clbDangChon = null;
            tbTieuDeForm.Text = "THÊM CLB MỚI";

            // Xóa form
            tbTenCLBForm.Clear();
            tbMoTaForm.Clear();
            tbChuNhiemForm.Clear();
            tbLienHeForm.Clear();
            tbSDTForm.Clear();
            TaiQuanLy(null);
            borderThongBaoForm.Visibility = Visibility.Collapsed;

            // Hiện form, ẩn chi tiết
            borderFormCLB.Visibility = Visibility.Visible;
            borderChiTietCLB.Visibility = Visibility.Collapsed;
            borderHuongDan.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Mở form chỉnh sửa CLB đang chọn
        /// </summary>
        private void NutSuaCLB_Click(object sender, RoutedEventArgs e)
        {
            if (_clbDangChon == null || !DichVuPhanQuyen.QuanLyCLB(_clbDangChon)) return;

            _cheDoDForm = "Sua";
            tbTieuDeForm.Text = "CHỈNH SỬA CLB";

            // Điền thông tin CLB vào form
            tbTenCLBForm.Text = _clbDangChon.TenCLB;
            tbMoTaForm.Text = _clbDangChon.MoTa;
            tbChuNhiemForm.Text = _clbDangChon.ChuNhiem;
            tbLienHeForm.Text = _clbDangChon.Email;
            tbSDTForm.Text = _clbDangChon.SoDienThoai;
            TaiQuanLy(_clbDangChon.MaNguoiDungQuanLy);
            borderThongBaoForm.Visibility = Visibility.Collapsed;

            // Hiện form
            borderFormCLB.Visibility = Visibility.Visible;
            borderChiTietCLB.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Xử lý xóa CLB
        /// </summary>
        private void NutXoaCLB_Click(object sender, RoutedEventArgs e)
        {
            if (_clbDangChon == null) return;

            var ketQua = MessageBox.Show(
                string.Format("Ngừng hoạt động CLB '{0}'? Lịch sử được giữ lại.", _clbDangChon.TenCLB),
                "Xác nhận xóa CLB",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (ketQua == MessageBoxResult.Yes)
            {
                var result = _dichVuCLB.XoaCLBCoThongBao(_clbDangChon.MaCLB);
                if (result.ThanhCong)
                {
                    MessageBox.Show("Đã xóa CLB thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    borderChiTietCLB.Visibility = Visibility.Collapsed;
                    borderHuongDan.Visibility = Visibility.Visible;
                    _clbDangChon = null;
                    TaiDanhSachCLB(string.Empty);
                }
                else
                {
                    MessageBox.Show(result.ThongBao, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Lưu thông tin CLB (thêm mới hoặc chỉnh sửa)
        /// </summary>
        private void NutLuuCLB_Click(object sender, RoutedEventArgs e)
        {
            string tenCLB = tbTenCLBForm.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tenCLB))
            {
                HienThiThongBaoForm("Vui lòng nhập tên CLB!", false);
                return;
            }

            if (_cheDoDForm == "Them")
            {
                // Tạo CLB mới
                var clbMoi = new CauLacBo
                {
                    TenCLB = tenCLB,
                    MoTa = tbMoTaForm.Text?.Trim() ?? string.Empty,
                    ChuNhiem = tbChuNhiemForm.Text?.Trim() ?? string.Empty,
                    Email = tbLienHeForm.Text?.Trim() ?? string.Empty,
                    SoDienThoai = tbSDTForm.Text?.Trim() ?? string.Empty,
                    MaNguoiDungQuanLy = cbQuanLyForm.SelectedValue as string,
                    NgayThanhLap = DateTime.Now,
                    SoThanhVien = 0
                };

                var result = _dichVuCLB.ThemCLBCoThongBao(clbMoi);
                if (result.ThanhCong)
                {
                    HienThiThongBaoForm("✅ Thêm CLB mới thành công!", true);
                    TaiDanhSachCLB(string.Empty);
                }
                else
                {
                    HienThiThongBaoForm(result.ThongBao, false);
                }
            }
            else if (_cheDoDForm == "Sua" && _clbDangChon != null)
            {
                // Cập nhật thông tin CLB
                var updated = new CauLacBo
                {
                    MaCLB = _clbDangChon.MaCLB, TenCLB = tenCLB,
                    MoTa = tbMoTaForm.Text?.Trim() ?? string.Empty,
                    ChuNhiem = tbChuNhiemForm.Text?.Trim() ?? string.Empty,
                    Email = tbLienHeForm.Text?.Trim() ?? string.Empty,
                    SoDienThoai = tbSDTForm.Text?.Trim() ?? string.Empty,
                    MaNguoiDungQuanLy = DichVuPhanQuyen.LaAdmin ? cbQuanLyForm.SelectedValue as string : _clbDangChon.MaNguoiDungQuanLy,
                    PhongBan = _clbDangChon.PhongBan, DiaDiemHoatDong = _clbDangChon.DiaDiemHoatDong
                };
                var result = _dichVuCLB.CapNhatCLBCoThongBao(updated);
                if (result.ThanhCong)
                {
                    HienThiThongBaoForm("✅ Cập nhật CLB thành công!", true);
                    TaiDanhSachCLB(string.Empty);
                }
                else
                {
                    HienThiThongBaoForm(result.ThongBao, false);
                }
            }
        }

        /// <summary>
        /// Hiển thị thông báo trong form
        /// </summary>
        private void HienThiThongBaoForm(string thongBao, bool thanhCong)
        {
            tbThongBaoForm.Text = thongBao;
            borderThongBaoForm.Background = thanhCong
                ? new SolidColorBrush(Color.FromRgb(232, 245, 233))
                : new SolidColorBrush(Color.FromRgb(255, 235, 238));
            tbThongBaoForm.Foreground = thanhCong
                ? new SolidColorBrush(Color.FromRgb(27, 94, 32))
                : new SolidColorBrush(Color.FromRgb(198, 40, 40));
            borderThongBaoForm.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hủy và đóng form
        /// </summary>
        private void NutHuyForm_Click(object sender, RoutedEventArgs e)
        {
            borderFormCLB.Visibility = Visibility.Collapsed;

            if (_clbDangChon != null)
                borderChiTietCLB.Visibility = Visibility.Visible;
            else
                borderHuongDan.Visibility = Visibility.Visible;
        }
    }
}
