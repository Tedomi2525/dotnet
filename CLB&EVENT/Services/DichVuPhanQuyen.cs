using CLB_EVENT.Models;

namespace CLB_EVENT.Services
{
    public static class DichVuPhanQuyen
    {
        public static bool LaAdmin => DichVuNguoiDung.NguoiDungHienTai != null &&
            DichVuNguoiDung.NguoiDungHienTai.TrangThaiHoatDong &&
            DichVuNguoiDung.NguoiDungHienTai.VaiTro == "QuanLyAdmin";

        public static bool LaQuanLy => LaAdmin || (DichVuNguoiDung.NguoiDungHienTai != null &&
            DichVuNguoiDung.NguoiDungHienTai.TrangThaiHoatDong &&
            DichVuNguoiDung.NguoiDungHienTai.VaiTro == "QuanLyCLB");

        public static bool QuanLyCLB(CauLacBo clb) => clb != null && (LaAdmin ||
            (LaQuanLy && clb.MaNguoiDungQuanLy == DichVuNguoiDung.NguoiDungHienTai.MaNguoiDung));

        public static bool LaNguoiDungHienTai(string maNguoiDung) =>
            DichVuNguoiDung.NguoiDungHienTai != null &&
            DichVuNguoiDung.NguoiDungHienTai.TrangThaiHoatDong &&
            DichVuNguoiDung.NguoiDungHienTai.MaNguoiDung == maNguoiDung;
    }
}
