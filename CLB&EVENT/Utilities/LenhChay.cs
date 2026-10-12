using System;
using System.Windows.Input;

namespace CLB_EVENT.Utilities
{
    /// <summary>
    /// Lớp triển khai ICommand để ràng buộc hành động từ View vào ViewModel
    /// theo mô hình MVVM (Model-View-ViewModel)
    /// </summary>
    public class LenhChay : ICommand
    {
        // Hàm thực thi lệnh
        private readonly Action<object> _thucThi;

        // Hàm kiểm tra có thể thực thi hay không
        private readonly Func<object, bool> _coTheThucThi;

        /// <summary>
        /// Khởi tạo LenhChay với hành động và điều kiện tùy chọn
        /// </summary>
        /// <param name="thucThi">Hành động được thực thi khi lệnh được gọi</param>
        /// <param name="coTheThucThi">Hàm kiểm tra điều kiện thực thi (tùy chọn)</param>
        public LenhChay(Action<object> thucThi, Func<object, bool> coTheThucThi = null)
        {
            _thucThi = thucThi ?? throw new ArgumentNullException(nameof(thucThi));
            _coTheThucThi = coTheThucThi;
        }

        /// <summary>
        /// Khởi tạo LenhChay đơn giản không có tham số
        /// </summary>
        /// <param name="thucThi">Hành động đơn giản</param>
        public LenhChay(Action thucThi)
            : this(_ => thucThi())
        {
        }

        /// <summary>
        /// Sự kiện kích hoạt khi điều kiện thực thi thay đổi
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Kiểm tra xem lệnh có thể thực thi hay không
        /// </summary>
        public bool CanExecute(object thamSo) =>
            _coTheThucThi == null || _coTheThucThi(thamSo);

        /// <summary>
        /// Thực thi lệnh
        /// </summary>
        public void Execute(object thamSo) => _thucThi(thamSo);

        /// <summary>
        /// Thông báo UI kiểm tra lại điều kiện CanExecute
        /// </summary>
        public void ThongBaoCoTheThucThiThayDoi() =>
            CommandManager.InvalidateRequerySuggested();
    }
}
