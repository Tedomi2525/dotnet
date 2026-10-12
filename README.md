# CLB & Event

Ứng dụng Windows quản lý câu lạc bộ và sự kiện sinh viên, viết bằng C# WPF (.NET Framework 4.7.2), lưu dữ liệu trên MySQL Aiven.

## Chức năng

- Đăng nhập, đăng ký tài khoản và phân quyền quản trị viên, quản lý CLB, sinh viên.
- Quản lý CLB, nộp đơn tham gia và xét duyệt thành viên.
- Quản lý sự kiện, đăng ký/hủy tham gia và xem lịch.
- Đăng, sửa, ẩn và đọc tin tức, tìm kiếm và lọc theo danh mục/ngày.

## Chạy dự án

1. Cài Visual Studio với workload **.NET desktop development** và .NET Framework 4.7.2 targeting pack.
2. Clone repo, mở `CLB&EVENT.sln` và restore các gói NuGet.
3. Làm theo [hướng dẫn cấu hình Aiven](AIVEN.md) để nhập mật khẩu MySQL trên máy của bạn.
4. Nhấn **F5** để chạy ứng dụng.

Mật khẩu Aiven được mã hóa bằng Windows DPAPI và lưu trong file `aiven.local.secret` trên máy. File mật khẩu, thư mục build và thiết lập Visual Studio cá nhân được loại khỏi Git. Chứng chỉ CA công khai `ca.pem` được giữ trong dự án để xác thực SSL.

Các tài khoản mẫu và lệnh kiểm tra được mô tả trong [AIVEN.md](AIVEN.md).
