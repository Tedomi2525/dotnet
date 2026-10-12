using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using CLB_EVENT;
using CLB_EVENT.Models;
using CLB_EVENT.Services;

class FunctionalChecks
{
    static int checks;
    static readonly DichVuNguoiDung users = new DichVuNguoiDung();
    static readonly DichVuCLB clubs = new DichVuCLB();
    static readonly DichVuSuKien events = new DichVuSuKien();
    static readonly DichVuTinTuc news = new DichVuTinTuc();
    static void Assert(bool value, string name) { checks++; if (!value) throw new Exception("FAIL: " + name); Console.WriteLine("PASS: " + name); }
    static object Field(object view, string name) => view.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(view);
    static object Invoke(object view, string name, params object[] args) => view.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke(view, args);
    static void Login(string role) { users.DangXuat(); Assert(users.DangNhap(role == "admin" ? "admin" : role == "manager" ? "tuanclb" : "binghan", role == "admin" ? "Admin@123" : role == "manager" ? "QuanLy@123" : "SinhVien@123") != null, "Login " + role); }
    static SuKien Event(string club = "CLB001", int capacity = 2) => new SuKien {
        TenSuKien = "Sự kiện kiểm tra", MaCLB = club, DiaDiem = "Phòng A",
        ThoiGianBatDau = DateTime.Now.AddDays(3), ThoiGianKetThuc = DateTime.Now.AddDays(3).AddHours(2),
        SoLuongToiDa = capacity, LoaiSuKien = "HộiThảo" };
    static CauLacBo ClubUpdate(CauLacBo source, string name) => new CauLacBo {
        MaCLB=source.MaCLB, TenCLB=name, Email=source.Email, SoDienThoai=source.SoDienThoai,
        ChuTichCLB=source.ChuTichCLB, MaNguoiDungQuanLy=source.MaNguoiDungQuanLy,
        MoTa=source.MoTa, PhongBan=source.PhongBan, DiaDiemHoatDong=source.DiaDiemHoatDong };
    static int NewsCount(TrangTinTuc view) => ((ItemsControl)Field(view,"dsTinTrai")).Items.Count + ((ItemsControl)Field(view,"dsTinPhai")).Items.Count;
    static void Snapshot(FrameworkElement view, string path)
    {
        view.Measure(new Size(1000,650)); view.Arrange(new Rect(0,0,1000,650)); view.UpdateLayout();
        System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        view.UpdateLayout();
        var bitmap = new RenderTargetBitmap(1000,650,96,96,PixelFormats.Pbgra32); bitmap.Render(view);
        var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using(var stream=File.Create(path)) encoder.Save(stream);
    }
    static Window Form(string name, object item) => (Window)Activator.CreateInstance(typeof(App).Assembly.GetType("CLB_EVENT." + name),
        BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,null,new object[]{item},null);
    static StackPanel Fields(Window window) => (StackPanel)((ScrollViewer)window.Content).Content;
    static bool SaveForm(Window dialog)
    {
        Exception failure = null;
        dialog.ShowInTaskbar = false; dialog.Opacity = 0;
        dialog.Loaded += (s,e) => dialog.Dispatcher.BeginInvoke(new Action(() => {
            try {
                var buttons = Fields(dialog).Children.OfType<WrapPanel>().Last().Children.OfType<Button>();
                buttons.First(b => (string)b.Content == "Lưu").RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            } catch(Exception ex) { failure = ex; }
            if (dialog.IsVisible) dialog.Close();
        }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        bool result = dialog.ShowDialog() == true;
        if (failure != null) throw failure;
        return result;
    }
    [STAThread] static int Main(string[] args)
    {
        CLB_EVENT.DuLieu.KhoDuLieuAiven.DungBoNhoChoKiemThu();
        var app = new App(); app.InitializeComponent(); app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        try {
            Login("student");
            foreach(var t in new[]{typeof(CuaSoDangNhap),typeof(CuaSoDangKy),typeof(CuaSoChinh),typeof(TrangChu),typeof(TrangTinTuc),typeof(TrangDangKyCLBForm),typeof(TrangDangKySuKien),typeof(TrangThoiGianSuKien),typeof(TrangQuanLyCLB),typeof(TrangDuyetDonCLB),typeof(TrangQuanLySuKien),typeof(TrangQuanLyTinTuc)})
            {
                var view=Activator.CreateInstance(t); Assert(view != null,"Construct " + t.Name);
                if(view is Window) ((Window)view).Close();
            }
            var input = new NguoiDung {HoTen="Kiểm tra", TenDangNhap="check", Email="invalid"};
            Assert(!users.DangKyTaiKhoan(input,"GoodPass123").ThanhCong,"Reject invalid email");
            input.Email="check@example.com";
            Assert(!users.DangKyTaiKhoan(input,"weak").ThanhCong,"Reject weak password at service");
            input.SoDienThoai="abc";
            Assert(!users.DangKyTaiKhoan(input,"GoodPass123").ThanhCong,"Reject invalid phone");
            input.SoDienThoai="0901234567";
            Assert(users.DangKyTaiKhoan(input,"GoodPass123").ThanhCong,"Register valid account");
            Assert(!users.DangKyTaiKhoan(new NguoiDung{HoTen="Test",TenDangNhap="CHECK",Email="other@example.com"},"GoodPass123").ThanhCong,"Reject duplicate username");
            Assert(!users.DangKyTaiKhoan(new NguoiDung{HoTen="Test",TenDangNhap="other",Email="CHECK@example.com"},"GoodPass123").ThanhCong,"Reject duplicate email");

            Assert(!clubs.ThemCLB(new CauLacBo{TenCLB="Unauthorized"}),"Student cannot create club");
            Assert(!events.ThemSuKien(Event()).ThanhCong,"Student cannot create event");
            Assert(!news.LuuTin(new TinTuc{TieuDe="Unauthorized",NoiDung="Text",DanhMuc="ThôngBáo",MaCLB="CLB001"},true).ThanhCong,"Student cannot publish news");
            Assert(!clubs.DangKyVaoCLB(new DangKyCLB{MaNguoiDung="ND003",MaCLB="CLB001",LyDo="Test"}).ThanhCong,"Reject impersonated club application");
            Assert(!clubs.DangKyVaoCLB(new DangKyCLB{MaNguoiDung="ND002",MaCLB="missing",LyDo="Test"}).ThanhCong,"Reject nonexistent club");
            var club=clubs.TimCLBTheoMa("CLB001"); int members=club.SoThanhVien;
            var application=new DangKyCLB{MaNguoiDung="ND002",MaCLB="CLB001",LyDo="Muốn học lập trình",KyNang="C#",ThoiGianRanh="Thứ 7"};
            Assert(clubs.DangKyVaoCLB(application).ThanhCong,"Submit club application");
            Assert(application.Email == users.TimNguoiDungTheoMa("ND002").Email && application.LopHoc=="CNTT-K20","Application keeps contact details");
            Assert(club.SoThanhVien==members,"Pending application does not add member");
            Assert(!clubs.XuLyDonDangKy(application.MaDangKy,true).ThanhCong,"Student cannot approve applications");
            Assert(!clubs.DangKyVaoCLB(new DangKyCLB{MaNguoiDung="ND002",MaCLB="CLB001",LyDo="Duplicate"}).ThanhCong,"Reject duplicate pending application");
            Login("manager");
            Assert(clubs.LayCLBDuocQuanLy().Count==1 && clubs.LayCLBDuocQuanLy()[0].MaCLB=="CLB001","Manager sees assigned club only");
            Assert(clubs.XuLyDonDangKy(application.MaDangKy,true).ThanhCong,"Assigned manager can approve");
            Assert(club.SoThanhVien==members+1,"Approval adds exactly one member");
            Assert(!clubs.XuLyDonDangKy(application.MaDangKy,true).ThanhCong && club.SoThanhVien==members+1,"Repeated approval cannot add member");
            Assert(!clubs.CapNhatCLB(ClubUpdate(clubs.TimCLBTheoMa("CLB002"),"Unauthorized")),"Manager cannot edit another club");
            Assert(!events.ThemSuKien(Event("CLB002")).ThanhCong,"Manager cannot create other club's event");
            Assert(!clubs.XoaCLB("CLB001"),"Manager cannot dissolve club");
            Login("student");
            var history = new TrangDangKyCLBForm();
            var historyItem=((ItemsControl)Field(history,"dsDangKyCLB")).Items[0];
            Assert((string)historyItem.GetType().GetProperty("NhanTrangThai").GetValue(historyItem,null)=="✅ Đã chấp nhận","Approved application label is correct");

            var reject=new DangKyCLB{MaNguoiDung="ND002",MaCLB="CLB002",LyDo="Test"};
            Assert(clubs.DangKyVaoCLB(reject).ThanhCong,"Submit second club application");
            Login("manager"); Assert(!clubs.XuLyDonDangKy(reject.MaDangKy,true).ThanhCong,"Manager cannot approve another club's application");
            Login("admin"); Assert(clubs.XuLyDonDangKy(reject.MaDangKy,false).ThanhCong,"Admin can reject");
            Login("student"); Assert(clubs.DangKyVaoCLB(new DangKyCLB{MaNguoiDung="ND002",MaCLB="CLB002",LyDo="Updated reason"}).ThanhCong,"Resubmit rejected application");

            Login("admin");
            string before=club.TenCLB;
            Assert(!clubs.CapNhatCLBCoThongBao(ClubUpdate(club,clubs.TimCLBTheoMa("CLB002").TenCLB)).ThanhCong && club.TenCLB==before,"Rejected club rename leaves data unchanged");
            Assert(!clubs.XoaCLBCoThongBao("CLB004").ThanhCong,"Cannot dissolve club with future event");
            Assert(!clubs.XoaCLBCoThongBao("CLB002").ThanhCong,"Cannot dissolve club with pending application");
            var inactive=new CauLacBo{TenCLB="Inactive test"};
            Assert(clubs.ThemCLB(inactive) && clubs.XoaCLB(inactive.MaCLB),"Dissolve club without active work");
            Login("student"); Assert(!clubs.DangKyVaoCLB(new DangKyCLB{MaNguoiDung="ND002",MaCLB=inactive.MaCLB,LyDo="Test"}).ThanhCong,"Reject inactive club application");

            Login("manager");
            var invalid=Event(); invalid.ThoiGianBatDau=DateTime.Now.AddDays(-2);
            Assert(!events.ThemSuKien(invalid).ThanhCong,"Reject past event");
            invalid=Event(); invalid.ThoiGianKetThuc=invalid.ThoiGianBatDau.AddHours(-1);
            Assert(!events.ThemSuKien(invalid).ThanhCong,"Reject inverted event dates");
            invalid=Event(capacity:0); Assert(!events.ThemSuKien(invalid).ThanhCong,"Reject zero capacity");
            var sk=Event(capacity:1); Assert(events.ThemSuKien(sk).ThanhCong,"Manager creates own event");
            Login("student");
            Assert(!events.DangKySuKien("ND003",sk.MaSuKien,"").ThanhCong,"Reject event impersonation");
            Assert(events.DangKySuKien("ND002",sk.MaSuKien,"Test note").ThanhCong && sk.SoLuongDaDangKy==1,"Register last available spot");
            Assert(!events.DangKySuKien("ND002",sk.MaSuKien,"").ThanhCong && sk.SoLuongDaDangKy==1,"Duplicate registration does not change count");
            var registration=events.LayDangKyCuaNguoiDung("ND002").First(d=>d.MaSuKien==sk.MaSuKien);
            Assert(registration.HoTen!=null && registration.Email!=null,"Event registration keeps contact details");
            Login("manager"); Assert(!events.DangKySuKien("ND003",sk.MaSuKien,"").ThanhCong,"Full event rejects another user");
            var smaller=Event(capacity:0); smaller.MaSuKien=sk.MaSuKien;
            Assert(!events.CapNhatSuKien(smaller).ThanhCong && sk.SoLuongToiDa==1,"Cannot reduce capacity below bookings");
            Login("student");
            Assert(events.HuyDangKy(registration.MaDangKy,"ND002").ThanhCong && sk.SoLuongDaDangKy==0,"Cancel restores available spot");
            Assert(!events.HuyDangKy(registration.MaDangKy,"ND002").ThanhCong && sk.SoLuongDaDangKy==0,"Repeated cancellation does not reduce count");
            Assert(events.DangKySuKien("ND002",sk.MaSuKien,"").ThanhCong,"Register again after cancellation");
            var registrationPage=new TrangDangKySuKien();
            Invoke(registrationPage,"ChonSuKien",sk);
            Assert(!((Button)Field(registrationPage,"nutDangKySuKien")).IsEnabled,"Already registered button disabled");
            Login("manager"); Assert(events.HuySuKien(sk.MaSuKien).ThanhCong,"Manager cancels own event");
            Login("student");
            Assert(!events.DangKySuKien("ND002",sk.MaSuKien,"").ThanhCong,"Canceled event cannot accept registration");
            Assert(events.LayDangKyCuaNguoiDung("ND002").First(d=>d.MaSuKien==sk.MaSuKien).TrangThai=="SựKiệnĐãHủy","Event cancellation updates participant history");
            registrationPage.LamMoi();
            Assert(Field(registrationPage,"_suKienDangChon")==null,"Refresh clears canceled event selection");

            Login("manager"); var live=Event(); Assert(events.ThemSuKien(live).ThanhCong,"Create event for time progression");
            Login("student"); Assert(events.DangKySuKien("ND002",live.MaSuKien,"").ThanhCong,"Register before start");
            var liveEntry=events.LayDangKyCuaNguoiDung("ND002").First(d=>d.MaSuKien==live.MaSuKien);
            live.ThoiGianBatDau=DateTime.Now.AddMinutes(-1);
            Assert(!events.DangKySuKien("ND002",live.MaSuKien,"").ThanhCong && live.TrangThai=="ĐangDiễnRa","Time progression closes registration");
            Assert(!events.HuyDangKy(liveEntry.MaDangKy,"ND002").ThanhCong,"Cannot cancel after event starts");

            var newsPage=new TrangTinTuc();
            foreach(var category in new[]{"rbThongBao","rbSuKien","rbHoatDong"}) {
                ((RadioButton)Field(newsPage,category)).IsChecked=true;
                Assert(NewsCount(newsPage)==2,"News category returns matching articles " + category);
            }
            ((RadioButton)Field(newsPage,"rbThongBao")).IsChecked=true; newsPage.TimKiem("Hackathon");
            Assert(NewsCount(newsPage)==0,"Search respects selected category");
            newsPage.TimKiem("");
            ((DatePicker)Field(newsPage,"dpTuNgay")).SelectedDate=DateTime.Today.AddDays(-2);
            ((DatePicker)Field(newsPage,"dpDenNgay")).SelectedDate=DateTime.Today.AddDays(-2);
            newsPage.LamMoi(); Assert(NewsCount(newsPage)==1,"News date boundaries are inclusive");
            ((DatePicker)Field(newsPage,"dpTuNgay")).SelectedDate=DateTime.Today;
            newsPage.LamMoi(); Assert(NewsCount(newsPage)==0 && ((TextBlock)Field(newsPage,"tbKetQua")).Text.Contains("Ngày bắt đầu"),"Reject reversed news date range");
            Login("manager");
            var article=new TinTuc{TieuDe="Published test",NoiDung="Full article",DanhMuc="CLB",MaCLB="CLB001"};
            Assert(news.LuuTin(article,true).ThanhCong,"Manager publishes assigned club news");
            var published=news.LayDanhSachTinTuc().First(t=>t.TieuDe==article.TieuDe); int views=published.LuotXem;
            Assert(news.DocTin(published.MaTinTuc).NoiDung=="Full article" && published.LuotXem==views+1,"Reading article gives full text and increments views");
            Assert(!news.LuuTin(new TinTuc{MaTinTuc="TT004",TieuDe="Bad",NoiDung="Bad",DanhMuc="CLB",MaCLB="CLB001"},false).ThanhCong,"Manager cannot edit other club news");
            Assert(news.AnTin(published.MaTinTuc).ThanhCong && news.DocTin(published.MaTinTuc)==null,"Hidden news cannot be read");
            Assert(!news.LayDanhSachTinTuc().Any(t=>t.MaTinTuc==published.MaTinTuc),"Hidden news removed from public list");

            var month=DateTime.Today.AddMonths(2); var boundary=new DateTime(month.Year,month.Month,1);
            var multi=Event(); multi.ThoiGianBatDau=boundary.AddDays(-1).AddHours(8); multi.ThoiGianKetThuc=boundary.AddDays(2).AddHours(12);
            Assert(events.ThemSuKien(multi).ThanhCong,"Create event spanning month boundary");
            var calendar=new TrangThoiGianSuKien();
            var marked=(HashSet<int>)Invoke(calendar,"LayNgayCoSuKien",boundary.Month,boundary.Year);
            Assert(marked.Contains(1) && marked.Contains(2) && marked.Contains(3),"Calendar marks every covered day across months");
            Assert(events.LaySuKienTheoThang(boundary.Month,boundary.Year).Any(e=>e.MaSuKien==multi.MaSuKien),"Month query includes overlapping event");
            Assert(clubs.CapNhatCLB(ClubUpdate(club,"CLB CNTT cập nhật")) && events.LaySuKienTheoMa(multi.MaSuKien).TenCLB=="CLB CNTT cập nhật","Club rename updates event display names");

            Login("admin");
            var main=new CuaSoChinh(); var home=(TrangChu)Field(main,"_trangChu"); int count=int.Parse(((TextBlock)Field(home,"tbTongCLB")).Text);
            Assert(clubs.ThemCLB(new CauLacBo{TenCLB="Refresh test"}),"Admin creates club");
            Invoke(main,"ChuyenDenTrangChu");
            Assert(int.Parse(((TextBlock)Field(home,"tbTongCLB")).Text)==count+1,"Returning home refreshes statistics");
            Assert(((ItemsControl)Field(home,"dsHoatDong")).Items.Count>0,"Home activity list populated");
            main.Close();
            var eventForm = Form("FormSuKien",null);
            var eventFields = Fields(eventForm).Children.OfType<TextBox>().ToArray();
            eventFields[0].Text = "Sự kiện từ form"; eventFields[1].Text = "Phòng kiểm tra";
            Assert(SaveForm(eventForm) && events.LayDanhSachSuKien().Any(s=>s.TenSuKien=="Sự kiện từ form"),"Save event dialog through UI button");
            var saved = events.LayDanhSachSuKien().First(s=>s.TenSuKien=="Sự kiện từ form");
            var eventEdit = Form("FormSuKien",saved);
            Fields(eventEdit).Children.OfType<TextBox>().First().Text = "Sự kiện từ form đã sửa";
            Assert(SaveForm(eventEdit) && events.LaySuKienTheoMa(saved.MaSuKien).TenSuKien=="Sự kiện từ form đã sửa","Edit event through UI button");
            var badEventForm = Form("FormSuKien",null);
            var badFields = Fields(badEventForm).Children.OfType<TextBox>().ToArray();
            badFields[0].Text = "Sự kiện không hợp lệ"; badFields[1].Text="Phòng A"; badFields[2].Text="25:00";
            Assert(!SaveForm(badEventForm) && !events.LayDanhSachSuKien().Any(s=>s.TenSuKien=="Sự kiện không hợp lệ"),"Invalid clock time cannot save dialog");
            var newsForm = Form("FormTinTuc",null);
            var newsFields=Fields(newsForm).Children.OfType<TextBox>().ToArray();
            newsFields[0].Text="Tin từ form"; newsFields[2].Text="Nội dung đầy đủ";
            Assert(SaveForm(newsForm) && news.LayDanhSachTinTuc().Any(t=>t.TieuDe=="Tin từ form"),"Publish news through UI button");
            var outputs=args.Length>0?args[0]:Path.GetTempPath();
            Snapshot(new TrangDuyetDonCLB(),Path.Combine(outputs,"approval.png"));
            Snapshot(new TrangQuanLySuKien(),Path.Combine(outputs,"events.png"));
            Snapshot(new TrangTinTuc(),Path.Combine(outputs,"news.png"));
            Snapshot(new TrangQuanLyCLB(),Path.Combine(outputs,"clubs.png"));
            foreach(var name in new[]{"CLB_EVENT.FormSuKien","CLB_EVENT.FormTinTuc"}) {
                var dialog=(Window)Activator.CreateInstance(typeof(App).Assembly.GetType(name),BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,null,new object[]{null},null);
                Snapshot((FrameworkElement)dialog.Content,Path.Combine(outputs,name.EndsWith("FormSuKien")?"event-form.png":"news-form.png")); dialog.Close();
            }
            Assert(true,"Render new views and edit dialogs");
            Console.WriteLine("RESULT: " + checks + " checks passed.");
            app.Shutdown(); return 0;
        } catch(Exception ex) { Console.Error.WriteLine(ex.GetBaseException()); app.Shutdown(); return 1; }
    }
}
