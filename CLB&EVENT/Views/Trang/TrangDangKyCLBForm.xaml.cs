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
    /// Form đăng ký CLB: sinh viên điền đơn để xin vào câu lạc bộ
    /// </summary>
    public partial class TrangDangKyCLBForm : UserControl
    {
        // Các dịch vụ cần dùng
        private readonly DichVuCLB _dichVuCLB;

        public TrangDangKyCLBForm()
        {
            InitializeComponent();
            _dichVuCLB = new DichVuCLB();

            // Điền thông tin người dùng hiện tại
            DienThongTinCaNhan();

            // Tải danh sách CLB
            TaiDanhSachCLB();

            // Tải lịch sử đăng ký CLB
            TaiLichSuDangKy();
        }

        /// <summary>
        /// Tự động điền thông tin cá nhân từ tài khoản đang đăng nhập
        /// </summary>
        private void DienThongTinCaNhan()
        {
            var nguoiDung = DichVuNguoiDung.NguoiDungHienTai;
            if (nguoiDung == null) return;

            tbHoTen.Text = nguoiDung.HoTen;
            tbEmail.Text = nguoiDung.Email;
            tbSoDienThoai.Text = nguoiDung.SoDienThoai;
            tbLopHoc.Text = nguoiDung.LopHoc;
        }

        /// <summary>
        /// Tải danh sách CLB vào ComboBox
        /// </summary>
        private void TaiDanhSachCLB()
        {
            var danhSachCLB = _dichVuCLB.LayDanhSachCLB()
                .Select(c => c.TenCLB)
                .ToList();

            // Thêm mục mặc định
            danhSachCLB.Insert(0, "-- Chọn CLB --");
            cbChonCLB.ItemsSource = danhSachCLB;
            cbChonCLB.SelectedIndex = 0;
        }

        /// <summary>
        /// Tải và hiển thị lịch sử đăng ký CLB của người dùng
        /// </summary>
        private void TaiLichSuDangKy()
        {
            var nguoiDung = DichVuNguoiDung.NguoiDungHienTai;
            if (nguoiDung == null) return;

            // Lấy tất cả đơn đăng ký CLB của người dùng
            var lichSu = _dichVuCLB.LayDangKyTheoNguoiDung(nguoiDung.MaNguoiDung)
                .Select(dk =>
                {
                    // Xác định màu và nhãn theo trạng thái
                    string mauTrangThai;
                    switch (dk.TrangThai)
                    {
                        case "ĐãDuyệt":
                            mauTrangThai = "#2E7D32";
                            break;
                        case "TừChối":
                            mauTrangThai = "#C62828";
                            break;
                        default:
                            mauTrangThai = "#F57C00";
                            break;
                    }

                    string nhanTrangThai;
                    switch (dk.TrangThai)
                    {
                        case "ĐãDuyệt":
                            nhanTrangThai = "✅ Đã chấp nhận";
                            break;
                        case "TừChối":
                            nhanTrangThai = "❌ Bị từ chối";
                            break;
                        default:
                            nhanTrangThai = "⏳ Đang chờ duyệt";
                            break;
                    }

                    return new
                    {
                        TenCLB = _dichVuCLB.TimCLBTheoMa(dk.MaCLB)?.TenCLB ?? dk.TenCLB,
                        NhanTrangThai = nhanTrangThai,
                        MauTrangThai = new SolidColorBrush((Color)ColorConverter.ConvertFromString(mauTrangThai)),
                        NgayDangKy = "📅 " + dk.NgayDangKy.ToString("dd/MM/yyyy")
                    };
                })
                .ToList();

            if (lichSu.Count > 0)
            {
                borderTrongLichSu.Visibility = Visibility.Collapsed;
                dsDangKyCLB.ItemsSource = lichSu;
            }
            else
            {
                borderTrongLichSu.Visibility = Visibility.Visible;
                dsDangKyCLB.ItemsSource = null;
            }
        }

        /// <summary>
        /// Hiển thị mô tả khi người dùng chọn một CLB trong danh sách
        /// </summary>
        private void CbChonCLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbChonCLB.SelectedIndex <= 0)
            {
                borderMoTaCLB.Visibility = Visibility.Collapsed;
                return;
            }

            string tenCLBChon = cbChonCLB.SelectedItem?.ToString() ?? string.Empty;
            var clb = _dichVuCLB.LayCLBTheoTen(tenCLBChon);

            if (clb != null)
            {
                tbMoTaCLB.Text = "📝 " + clb.MoTa;
                tbChuNhiemCLB.Text = "👨‍💼 Chủ nhiệm: " + clb.ChuNhiem;
                borderMoTaCLB.Visibility = Visibility.Visible;
            }
            else
            {
                borderMoTaCLB.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Xử lý khi nhấn nút gửi đơn đăng ký CLB
        /// </summary>
        private void NutGuiDon_Click(object sender, RoutedEventArgs e)
        {
            var nguoiDung = DichVuNguoiDung.NguoiDungHienTai;
            if (nguoiDung == null) return;

            // Kiểm tra đã chọn CLB chưa
            if (cbChonCLB.SelectedIndex <= 0)
            {
                HienThiThongBao("Vui lòng chọn CLB muốn đăng ký!", false);
                cbChonCLB.Focus();
                return;
            }

            // Kiểm tra đã điền lý do chưa
            string lyDo = tbLyDo.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(lyDo))
            {
                HienThiThongBao("Vui lòng điền lý do muốn tham gia CLB!", false);
                tbLyDo.Focus();
                return;
            }

            string tenCLBChon = cbChonCLB.SelectedItem?.ToString() ?? string.Empty;
            var clb = _dichVuCLB.LayCLBTheoTen(tenCLBChon);

            if (clb == null)
            {
                HienThiThongBao("Không tìm thấy thông tin CLB. Vui lòng thử lại!", false);
                return;
            }

            // Xây dựng chuỗi ngày rảnh
            var ngayRanh = new System.Text.StringBuilder();
            if (chkHai.IsChecked == true) ngayRanh.Append("Thứ 2, ");
            if (chkBa.IsChecked == true) ngayRanh.Append("Thứ 3, ");
            if (chkTu.IsChecked == true) ngayRanh.Append("Thứ 4, ");
            if (chkNam.IsChecked == true) ngayRanh.Append("Thứ 5, ");
            if (chkSau.IsChecked == true) ngayRanh.Append("Thứ 6, ");
            if (chkBay.IsChecked == true) ngayRanh.Append("Thứ 7, ");
            if (chkChuNhat.IsChecked == true) ngayRanh.Append("Chủ nhật, ");

            // Tạo đơn đăng ký CLB
            var dongkyMoi = new DangKyCLB
            {
                MaNguoiDung = nguoiDung.MaNguoiDung,
                TenNguoiDung = nguoiDung.HoTen,
                MaCLB = clb.MaCLB,
                TenCLB = clb.TenCLB,
                LyDo = lyDo,
                KyNang = tbKyNang.Text?.Trim() ?? string.Empty,
                ThoiGianRanh = ngayRanh.ToString().TrimEnd(' ', ','),
                NgayDangKy = DateTime.Now,
                TrangThai = "ChờDuyệt"
            };

            // Gửi đơn đăng ký qua dịch vụ
            var ketQua = _dichVuCLB.DangKyVaoCLB(dongkyMoi);

            if (ketQua.ThanhCong)
            {
                HienThiThongBao("✅ Gửi đơn đăng ký thành công! Vui lòng chờ Ban quản lý CLB xét duyệt.", true);

                // Reset form
                cbChonCLB.SelectedIndex = 0;
                tbLyDo.Clear();
                tbKyNang.Clear();
                chkHai.IsChecked = false;
                chkBa.IsChecked = false;
                chkTu.IsChecked = false;
                chkNam.IsChecked = false;
                chkSau.IsChecked = false;
                chkBay.IsChecked = false;
                chkChuNhat.IsChecked = false;

                // Cập nhật lịch sử
                TaiLichSuDangKy();
            }
            else
            {
                HienThiThongBao("❌ " + ketQua.ThongBao, false);
            }
        }

        /// <summary>
        /// Hiển thị thông báo kết quả
        /// </summary>
        private void HienThiThongBao(string thongBao, bool thanhCong)
        {
            tbThongBao.Text = thongBao;
            borderThongBao.Background = thanhCong
                ? new SolidColorBrush(Color.FromRgb(232, 245, 233))
                : new SolidColorBrush(Color.FromRgb(255, 235, 238));
            tbThongBao.Foreground = thanhCong
                ? new SolidColorBrush(Color.FromRgb(27, 94, 32))
                : new SolidColorBrush(Color.FromRgb(198, 40, 40));
            borderThongBao.Visibility = Visibility.Visible;
        }
    }
}
