$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Security
$password = Read-Host 'Mật khẩu MySQL Aiven mới' -AsSecureString
$pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($password)
try {
    $bytes = [Text.Encoding]::UTF8.GetBytes([Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer))
    $encrypted = [Security.Cryptography.ProtectedData]::Protect($bytes, $null, [Security.Cryptography.DataProtectionScope]::CurrentUser)
    $project = Join-Path (Split-Path $PSScriptRoot -Parent) 'CLB&EVENT'
    [IO.File]::WriteAllText((Join-Path $project 'aiven.local.secret'), [Convert]::ToBase64String($encrypted))
    # Cập nhật cả các bản build đã có trên máy này.
    $bin = Join-Path $project 'bin'
    if (Test-Path -LiteralPath $bin) {
        Get-ChildItem -LiteralPath $bin -Filter ClubEventManager.exe -Recurse | ForEach-Object {
            [IO.File]::WriteAllText((Join-Path $_.DirectoryName 'aiven.local.secret'), [Convert]::ToBase64String($encrypted))
        }
    }
    Write-Output 'Đã lưu mật khẩu được mã hóa cho tài khoản Windows hiện tại. Có thể chạy ứng dụng.'
}
finally {
    if ($bytes) { [Array]::Clear($bytes, 0, $bytes.Length) }
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer)
    $password.Dispose()
}
