$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Security
[Windows.Forms.Application]::EnableVisualStyles()
$project = Join-Path (Split-Path $PSScriptRoot -Parent) 'CLB&EVENT'
$form = New-Object Windows.Forms.Form
$form.Text = 'Cấu hình mật khẩu MySQL Aiven'
$form.Size = New-Object Drawing.Size(530, 230)
$form.StartPosition = 'CenterScreen'
$form.FormBorderStyle = 'FixedDialog'
$form.MaximizeBox = $false
$form.MinimizeBox = $false
$form.TopMost = $true
$label = New-Object Windows.Forms.Label
$label.Text = "Nhập mật khẩu MySQL hiện tại của tài khoản avnadmin từ Aiven.`r`nMật khẩu sẽ được mã hóa trên máy này và không gửi vào chat."
$label.AutoSize = $false
$label.Location = New-Object Drawing.Point(20, 18)
$label.Size = New-Object Drawing.Size(475, 48)
$passwordBox = New-Object Windows.Forms.TextBox
$passwordBox.UseSystemPasswordChar = $true
$passwordBox.Location = New-Object Drawing.Point(20, 80)
$passwordBox.Size = New-Object Drawing.Size(475, 26)
$save = New-Object Windows.Forms.Button
$save.Text = 'Lưu mật khẩu'
$save.Location = New-Object Drawing.Point(340, 128)
$save.Size = New-Object Drawing.Size(155, 32)
$form.Controls.AddRange(@($label, $passwordBox, $save))
$form.AcceptButton = $save
$save.Add_Click({
    if ([string]::IsNullOrWhiteSpace($passwordBox.Text)) { $passwordBox.Focus(); return }
    $bytes = $null
    try {
        $bytes = [Text.Encoding]::UTF8.GetBytes($passwordBox.Text)
        $encrypted = [Security.Cryptography.ProtectedData]::Protect($bytes, $null, [Security.Cryptography.DataProtectionScope]::CurrentUser)
        $text = [Convert]::ToBase64String($encrypted)
        [IO.File]::WriteAllText((Join-Path $project 'aiven.local.secret'), $text)
        $bin = Join-Path $project 'bin'
        if (Test-Path -LiteralPath $bin) {
            Get-ChildItem -LiteralPath $bin -Filter ClubEventManager.exe -Recurse | ForEach-Object {
                [IO.File]::WriteAllText((Join-Path $_.DirectoryName 'aiven.local.secret'), $text)
            }
        }
        $passwordBox.Clear()
        $form.Close()
    }
    catch { [Windows.Forms.MessageBox]::Show('Không lưu được cấu hình: ' + $_.Exception.GetType().Name) | Out-Null }
    finally { if ($bytes) { [Array]::Clear($bytes, 0, $bytes.Length) } }
})
[void]$form.ShowDialog()
$form.Dispose()
