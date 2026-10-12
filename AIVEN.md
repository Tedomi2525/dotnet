# Kết nối MySQL Aiven

Ứng dụng dùng MySqlConnector 2.6.2, database `defaultdb`, cổng `12906` trên dịch vụ `mysql-36196097-pjdotnet1.a.aivencloud.com`. Host, cổng, database và user nằm trong `CLB&EVENT/App.config`.

Kết nối dùng TLS 1.2 để tương thích .NET Framework 4.7.2, `SslMode=VerifyFull` và chứng chỉ `CLB&EVENT/ca.pem`. VerifyFull kiểm tra cả CA và tên máy chủ. [Tài liệu MySqlConnector](https://mysqlconnector.net/connection-options/)

## Cài đặt trên máy này

1. Mở PowerShell tại thư mục chứa solution, chạy một trong hai cách:

   ```powershell
   powershell -NoProfile -STA -ExecutionPolicy Bypass -File .\scripts\Configure-Aiven.ps1
   ```

   Hoặc nhập mật khẩu ngay trong terminal:

   ```powershell
   powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Set-AivenPassword.ps1
   ```

2. Nhập mật khẩu MySQL của `avnadmin` từ Aiven. Mật khẩu được mã hóa bằng Windows DPAPI cho tài khoản Windows hiện tại, lưu trong `CLB&EVENT/aiven.local.secret`. File này được bỏ qua trong Git và được copy khi build. Máy khác cần tự nhập lại mật khẩu.
3. Mở solution trong Visual Studio, cho NuGet restore và nhấn F5. Lần mở đầu, ứng dụng kết nối Aiven trước khi hiển thị đăng nhập. Nếu mất mạng hoặc sai mật khẩu, ứng dụng báo lỗi thay vì chạy với dữ liệu RAM.

Có thể ghi đè cấu hình bằng biến môi trường `AIVEN_DB_HOST`, `AIVEN_DB_PORT`, `AIVEN_DB_DATABASE`, `AIVEN_DB_USER`, `AIVEN_DB_CAFILE`, `AIVEN_DB_PASSWORD`. Ưu tiên biến môi trường, sau đó App.config và file mật khẩu mã hóa.

## Dữ liệu

Ứng dụng chỉ tạo các bảng `clbe_metadata`, `clbe_nguoi_dung`, `clbe_cau_lac_bo`, `clbe_su_kien`, `clbe_dang_ky_clb`, `clbe_dang_ky_su_kien`, `clbe_tin_tuc`. Không drop bảng hoặc xóa dữ liệu có sẵn khi khởi động.

Dữ liệu mẫu hiện có được đưa lên một lần khi toàn bộ sáu bảng nghiệp vụ trống. Sau đó đăng ký tài khoản, tạo/sửa CLB, nộp/duyệt đơn, tạo/sửa/hủy sự kiện, đăng ký/hủy tham gia, đăng/ẩn tin và lượt xem đều được lưu trong MySQL. Các số thành viên, người đăng ký và lượt xem ban đầu vẫn là số minh họa của dữ liệu mẫu.

Các tài khoản mẫu hiện có được giữ lại: `admin` / `Admin@123`, `binghan` / `SinhVien@123`, `tuanclb` / `QuanLy@123`.

Mỗi lần đọc service lấy snapshot nhất quán từ MySQL; mỗi thao tác ghi tải dữ liệu mới, kiểm tra quyền và điều kiện, rồi lưu những hàng thay đổi trong một transaction. Khóa hàng metadata ngăn hai máy cùng chiếm chỗ cuối hoặc duyệt đơn hai lần. Các bảng có khóa chính, khóa ngoại và unique tên đăng nhập/email/tên CLB. Đây là cách lưu phù hợp quy mô dự án hiện tại; thao tác service vẫn đồng bộ trên giao diện, còn kết nối khởi động chạy nền.

## Kiểm tra

```powershell
# Kiểm tra chức năng bằng dữ liệu RAM dành riêng cho test, không ghi lên Aiven
.\tests\Run-Checks.ps1

# Kiểm tra SSL và liệt kê bảng hiện có, chỉ đọc
.\tests\Run-AivenChecks.ps1 -Mode inspect

# Kiểm tra ghi/đọc, đồng thời từ hai process, và dọn đúng bản ghi kiểm thử
.\tests\Run-AivenChecks.ps1 -Mode full
```

Kiểm tra `full` dùng tài khoản mẫu nêu trên, tạo dữ liệu có mã ngẫu nhiên và xóa đúng các mã đó trong `finally`; không xóa dữ liệu mẫu hoặc dữ liệu của người dùng. Bộ kiểm tra cần Visual Studio/Build Tools với .NET desktop development và quyền kết nối mạng tới Aiven.
