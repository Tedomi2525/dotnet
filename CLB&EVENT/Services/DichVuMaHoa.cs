using System;
using System.Security.Cryptography;
using System.Text;

namespace CLB_EVENT.Services
{
    /// <summary>
    /// Dịch vụ mã hóa mật khẩu sử dụng hàm băm SHA-256
    /// Đây là phương pháp mã hóa một chiều an toàn cho dữ liệu mật khẩu
    /// </summary>
    public static class DichVuMaHoa
    {
        /// <summary>
        /// Mã hóa mật khẩu bằng thuật toán SHA-256
        /// SHA-256 là hàm băm một chiều, không thể giải mã ngược
        /// </summary>
        /// <param name="matKhau">Mật khẩu gốc cần mã hóa</param>
        /// <returns>Chuỗi mã hóa dạng HEX (64 ký tự)</returns>
        public static string MaHoaSHA256(string matKhau)
        {
            if (string.IsNullOrEmpty(matKhau))
                throw new ArgumentNullException(nameof(matKhau), "Mật khẩu không được để trống");

            // Khởi tạo đối tượng SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                // Chuyển chuỗi mật khẩu thành mảng byte dùng UTF-8
                byte[] mangByte = Encoding.UTF8.GetBytes(matKhau);

                // Tính toán giá trị băm
                byte[] bangBam = sha256.ComputeHash(mangByte);

                // Chuyển mảng byte thành chuỗi HEX
                StringBuilder chuoiKetQua = new StringBuilder();
                foreach (byte b in bangBam)
                {
                    // x2 = định dạng hex 2 chữ số
                    chuoiKetQua.Append(b.ToString("x2"));
                }

                return chuoiKetQua.ToString();
            }
        }

        /// <summary>
        /// So sánh mật khẩu nhập vào với mật khẩu đã mã hóa trong database
        /// </summary>
        /// <param name="matKhauNhap">Mật khẩu người dùng nhập</param>
        /// <param name="matKhauDaMaHoa">Mật khẩu đã được mã hóa lưu trong hệ thống</param>
        /// <returns>True nếu mật khẩu khớp</returns>
        public static bool XacThucMatKhau(string matKhauNhap, string matKhauDaMaHoa)
        {
            if (string.IsNullOrEmpty(matKhauNhap) || string.IsNullOrEmpty(matKhauDaMaHoa))
                return false;

            // Mã hóa mật khẩu nhập vào và so sánh
            string matKhauMaHoaMoi = MaHoaSHA256(matKhauNhap);
            return string.Equals(matKhauMaHoaMoi, matKhauDaMaHoa, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Kiểm tra độ mạnh của mật khẩu
        /// Mật khẩu phải có ít nhất 8 ký tự, có chữ hoa, chữ thường và số
        /// </summary>
        /// <param name="matKhau">Mật khẩu cần kiểm tra</param>
        /// <returns>True nếu mật khẩu đủ mạnh</returns>
        public static bool KiemTraDoManhMatKhau(string matKhau)
        {
            if (string.IsNullOrEmpty(matKhau) || matKhau.Length < 8)
                return false;

            bool coSo = false;
            bool coChuHoa = false;
            bool coChuThuong = false;

            foreach (char kyTu in matKhau)
            {
                if (char.IsDigit(kyTu)) coSo = true;
                if (char.IsUpper(kyTu)) coChuHoa = true;
                if (char.IsLower(kyTu)) coChuThuong = true;
            }

            return coSo && coChuHoa && coChuThuong;
        }
    }
}
