using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using CLB_EVENT;
using CLB_EVENT.DuLieu;
using CLB_EVENT.Models;
using CLB_EVENT.Services;
using MySqlConnector;

class AivenChecks
{
    static int checks;
    static void Assert(bool ok, string name) { if (!ok) throw new Exception(name); checks++; Console.WriteLine("PASS: " + name); }
    static MySqlConnection Open() { var c = new MySqlConnection(CauHinhAiven.TaoChuoiKetNoi()); c.Open(); return c; }
    static long Count(MySqlConnection c, string sql) { using(var cmd=new MySqlCommand(sql,c)) return Convert.ToInt64(cmd.ExecuteScalar()); }
    [STAThread] static int Main(string[] args)
    {
        try
        {
            if (args.Length > 0 && args[0] == "inspect")
            {
                using(var c=Open())
                {
                    Console.WriteLine("CONNECTED: " + c.Database + "; MySQL " + c.ServerVersion);
                    using(var cmd=new MySqlCommand("SHOW SESSION STATUS LIKE 'Ssl_cipher'",c))
                    using(var reader=cmd.ExecuteReader()) { reader.Read(); Console.WriteLine("TLS: " + reader.GetString(1)); }
                    using(var cmd=new MySqlCommand("SHOW TABLES",c))
                    using(var reader=cmd.ExecuteReader()) while(reader.Read()) Console.WriteLine("TABLE: " + reader.GetString(0));
                }
                return 0;
            }
            KhoDuLieuAiven.KhoiTao();
            var users=new DichVuNguoiDung(); var clubs=new DichVuCLB(); var events=new DichVuSuKien(); var news=new DichVuTinTuc();
            if(args.Length > 0 && args[0]=="join")
            {
                if(users.DangNhap(args[1],"AivenTest123") == null) return 3;
                var result=events.DangKySuKien(DichVuNguoiDung.NguoiDungHienTai.MaNguoiDung,args[2],"Aiven integration test");
                Console.WriteLine(result.ThanhCong ? "JOINED" : "FULL"); return result.ThanhCong ? 0 : 2;
            }
            Assert(users.DangNhap("admin","Admin@123") != null,"Login using persisted admin account");
            Assert(clubs.LayTatCaCLB().Count >= 6 && events.LayDanhSachSuKien().Count >= 6 && news.LayDanhSachTinTuc().Count >= 6,"Load seed data from Aiven");
            // Khởi tạo lần hai không seed thêm hoặc đổi ngày giờ dữ liệu.
            var seedDate=events.LaySuKienTheoMa("SK001").ThoiGianBatDau;
            KhoDuLieuAiven.KhoiTao();
            Assert(events.LaySuKienTheoMa("SK001").ThoiGianBatDau == seedDate,"Initialization is idempotent");
            var token=Guid.NewGuid().ToString("N");
            var first=new NguoiDung{HoTen="Kiểm thử Aiven",TenDangNhap="test_"+token,Email=token+"@example.com",LopHoc="Kiểm thử",SoDienThoai="0901234567"};
            var second=new NguoiDung{HoTen="Kiểm thử Aiven 2",TenDangNhap="test2_"+token,Email="second_"+token+"@example.com"};
            var club=new CauLacBo{TenCLB="Kiểm thử Aiven " + token,MaNguoiDungQuanLy="ND003",MoTa="Bản ghi kiểm thử sẽ được dọn sau khi chạy."};
            SuKien sk=null; string articleId=null;
            try
            {
                Assert(users.DangKyTaiKhoan(first,"AivenTest123").ThanhCong,"Persist a new account");
                Assert(users.DangKyTaiKhoan(second,"AivenTest123").ThanhCong,"Persist second account");
                Assert(clubs.ThemCLB(club),"Persist club through public alias API");
                sk=new SuKien{TenSuKien="Sự kiện kiểm thử " + token,MaCLB=club.MaCLB,DiaDiem="Phòng kiểm thử",LoaiSuKien="HộiThảo",
                    ThoiGianBatDau=DateTime.Today.AddDays(2).AddHours(8),ThoiGianKetThuc=DateTime.Today.AddDays(2).AddHours(10),SoLuongToiDa=1};
                Assert(events.ThemSuKien(sk).ThanhCong,"Persist event");
                var article=new TinTuc{TieuDe="Tin kiểm thử " + token,NoiDung="Nội dung tiếng Việt 🎉 ' ; SELECT",DanhMuc="CLB",MaCLB=club.MaCLB};
                Assert(news.LuuTin(article,true).ThanhCong,"Persist Vietnamese text and SQL metacharacters");
                articleId=news.LayDanhSachTinTuc().Single(t=>t.TieuDe==article.TieuDe).MaTinTuc;
                using(var c=Open())
                {
                    using(var cmd=new MySqlCommand("SELECT NoiDung FROM clbe_tin_tuc WHERE MaTinTuc=@id",c))
                    { cmd.Parameters.AddWithValue("@id",articleId); Assert((string)cmd.ExecuteScalar()==article.NoiDung,"UTF-8 round trip verified on separate SQL connection"); }
                }
                // Xác nhận DocTin thực sự ghi lượt xem, không chỉ tăng trong RAM.
                news.DocTin(articleId);
                using(var c=Open()) Assert(Count(c,"SELECT SUM(LuotXem) FROM clbe_tin_tuc WHERE MaTinTuc='"+articleId+"'")==1,"View count persisted");
                Assert(users.DangNhap(first.TenDangNhap,"AivenTest123") != null,"Login using newly persisted account");
                var application=new DangKyCLB{MaNguoiDung=first.MaNguoiDung,MaCLB=club.MaCLB,LyDo="Tham gia kiểm thử"};
                Assert(clubs.NopDonDangKyCLB(application).ThanhCong,"Persist club application through alias API");
                users.DangNhap("tuanclb","QuanLy@123");
                Assert(clubs.XuLyDonDangKy(application.MaDangKy,true).ThanhCong,"Persist approval and member counter together");
                Assert(!clubs.XuLyDonDangKy(application.MaDangKy,true).ThanhCong,"Repeated approval rejected using stored status");
                Assert(clubs.TimCLBTheoMa(club.MaCLB).SoThanhVien==1,"Member counter reads back as one");
                var p1=Join(first.TenDangNhap,sk.MaSuKien); var p2=Join(second.TenDangNhap,sk.MaSuKien);
                p1.WaitForExit(); p2.WaitForExit();
                var exits=new[]{p1.ExitCode,p2.ExitCode};
                Assert(exits.Count(e=>e==0)==1 && exits.Count(e=>e==2)==1,"Two independent processes cannot overbook the last seat");
                Assert(events.LaySuKienTheoMa(sk.MaSuKien).SoLuongDaDangKy==1,"Reload sees other process's booking");
                var winner=p1.ExitCode==0 ? first : second;
                users.DangNhap(winner.TenDangNhap,"AivenTest123");
                var booking=events.LayDangKyCuaNguoiDung(winner.MaNguoiDung).Single(d=>d.MaSuKien==sk.MaSuKien);
                Assert(events.HuyDangKy(booking.MaDangKy,winner.MaNguoiDung).ThanhCong,"Cancellation persisted");
                Assert(events.LaySuKienTheoMa(sk.MaSuKien).SoLuongDaDangKy==0,"Cancellation restores the stored seat");
                users.DangNhap("tuanclb","QuanLy@123");
                Assert(news.AnTin(articleId).ThanhCong && news.DocTin(articleId)==null,"Hidden news stays hidden after reload");
                var app=new App(); app.InitializeComponent(); app.ShutdownMode=ShutdownMode.OnExplicitShutdown;
                var window=new CuaSoChinh(); Assert(window!=null,"Construct main WPF window backed by Aiven"); window.Close(); app.Shutdown();
            }
            finally
            {
                // Dọn đúng ID vừa tạo; không xóa seed hoặc dữ liệu của người dùng.
                using(var c=Open()) using(var tx=c.BeginTransaction())
                {
                    using(var mutex=new MySqlCommand("SELECT Id FROM clbe_metadata WHERE Id=1 FOR UPDATE",c,tx)) mutex.ExecuteScalar();
                    string[] sql={"DELETE FROM clbe_dang_ky_su_kien WHERE MaSuKien=@event",
                        "DELETE FROM clbe_dang_ky_clb WHERE MaCLB=@club","DELETE FROM clbe_tin_tuc WHERE MaCLB=@club",
                        "DELETE FROM clbe_su_kien WHERE MaCLB=@club","DELETE FROM clbe_cau_lac_bo WHERE MaCLB=@club",
                        "DELETE FROM clbe_nguoi_dung WHERE MaNguoiDung IN (@user1,@user2)"};
                    foreach(var s in sql) using(var cmd=new MySqlCommand(s,c,tx))
                    { cmd.Parameters.AddWithValue("@event",sk?.MaSuKien ?? ""); cmd.Parameters.AddWithValue("@club",club.MaCLB);
                      cmd.Parameters.AddWithValue("@user1",first.MaNguoiDung); cmd.Parameters.AddWithValue("@user2",second.MaNguoiDung); cmd.ExecuteNonQuery(); }
                    tx.Commit();
                }
                Console.WriteLine("CLEANUP: test records removed.");
            }
            Console.WriteLine("RESULT: " + checks + " Aiven checks passed."); return 0;
        }
        catch(Exception ex) { Console.Error.WriteLine("FAIL: " + ex.GetType().Name + ": " + KhoDuLieuAiven.ThongBaoLoi(ex)); return 1; }
    }
    static Process Join(string user,string eventId)
    {
        var psi=new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().Location,"join " + user + " " + eventId)
            {UseShellExecute=false,CreateNoWindow=true,WindowStyle=ProcessWindowStyle.Hidden};
        return Process.Start(psi);
    }
}
