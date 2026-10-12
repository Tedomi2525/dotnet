using System;
using System.ComponentModel;

namespace CLB_EVENT.Utilities
{
    /// <summary>
    /// Lớp cơ sở cho tất cả ViewModel, triển khai INotifyPropertyChanged
    /// để hỗ trợ Data Binding trong WPF
    /// </summary>
    public abstract class ViewModelCoSo : INotifyPropertyChanged
    {
        /// <summary>
        /// Sự kiện thông báo khi thuộc tính thay đổi
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Phương thức kích hoạt sự kiện PropertyChanged
        /// </summary>
        /// <param name="tenThuocTinh">Tên thuộc tính bị thay đổi</param>
        protected void ThongBaoThuocTinhThayDoi(string tenThuocTinh = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tenThuocTinh));
        }

        /// <summary>
        /// Đặt giá trị thuộc tính và tự động thông báo nếu giá trị thay đổi
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của thuộc tính</typeparam>
        /// <param name="truong">Trường lưu giá trị</param>
        /// <param name="giaTri">Giá trị mới</param>
        /// <param name="tenThuocTinh">Tên thuộc tính</param>
        /// <returns>True nếu giá trị thực sự thay đổi</returns>
        protected bool DatGiaTri<T>(ref T truong, T giaTri, string tenThuocTinh = null)
        {
            if (Equals(truong, giaTri))
                return false;

            truong = giaTri;
            ThongBaoThuocTinhThayDoi(tenThuocTinh);
            return true;
        }
    }
}
