using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MySqlConnector;

namespace CLB_EVENT.DuLieu
{
    public static class CauHinhAiven
    {
        public static string TaoChuoiKetNoi()
        {
            string ThuocTinh(string ten) => Environment.GetEnvironmentVariable("AIVEN_DB_" + ten.ToUpperInvariant())
                ?? ConfigurationManager.AppSettings["Aiven." + ten];
            var password = Environment.GetEnvironmentVariable("AIVEN_DB_PASSWORD");
            if (string.IsNullOrEmpty(password))
            {
                var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "aiven.local.secret");
                if (!File.Exists(file))
                    throw new InvalidOperationException("Chưa có mật khẩu Aiven. Chạy scripts/Set-AivenPassword.ps1 trên máy này.");
                try
                {
                    password = Encoding.UTF8.GetString(ProtectedData.Unprotect(
                        Convert.FromBase64String(File.ReadAllText(file).Trim()), null, DataProtectionScope.CurrentUser));
                }
                catch (CryptographicException)
                {
                    throw new InvalidOperationException("Mật khẩu Aiven được mã hóa cho tài khoản Windows khác. Chạy lại scripts/Set-AivenPassword.ps1.");
                }
            }
            var ca = ThuocTinh("CaFile") ?? "ca.pem";
            if (!Path.IsPathRooted(ca)) ca = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ca);
            if (!File.Exists(ca)) throw new InvalidOperationException("Thiếu chứng chỉ ca.pem của Aiven.");
            return new MySqlConnectionStringBuilder
            {
                Server = ThuocTinh("Host"), Port = uint.Parse(ThuocTinh("Port") ?? "12906"),
                Database = ThuocTinh("Database"), UserID = ThuocTinh("User"), Password = password,
                SslMode = MySqlSslMode.VerifyFull, SslCa = ca,
                // .NET Framework 4.7.2 không hỗ trợ enum TLS 1.3 của các bản Windows mới.
                TlsVersion = "TLS 1.2",
                ConnectionTimeout = 10, DefaultCommandTimeout = 15,
                MaximumPoolSize = 5, CharacterSet = "utf8mb4", PersistSecurityInfo = false
            }.ConnectionString;
        }
    }
}
