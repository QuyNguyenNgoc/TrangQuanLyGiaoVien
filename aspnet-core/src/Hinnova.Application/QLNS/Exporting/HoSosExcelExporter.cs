using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Hinnova.DataExporting.Excel.EpPlus;
using Hinnova.QLNSDtos;
using Hinnova.Dto;
using Hinnova.Storage;

namespace Hinnova.QLNSExporting
{
    public class HoSosExcelExporter : EpPlusExcelExporterBase, IHoSosExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public HoSosExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetHoSoForViewDto> hoSos)
        {
            return CreateExcelPackage(
                "HoSos.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("HoSos"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("MaNhanVien"),
                        L("HoVaTen"),
                        L("AnhDaiDien"),
                        L("GioiTinhCode"),
                        L("NgaySinh"),
                        L("MSTCaNhan"),
                        L("DonViCongTacID"),
                        L("ViTriCongViecCode"),
                        L("DanToc"),
                        L("TonGiao"),
                        L("QuocTich"),
                        L("SoCMND"),
                        L("NgayCap"),
                        L("NoiCap"),
                        L("NgayHetHan"),
                        L("TrinhDoVanHoa"),
                        L("TrinhDoDaoTaoCode"),
                        L("NoiDaoTaoCode"),
                        L("Khoa"),
                        L("ChuyenNganh"),
                        L("NamTotNghiep"),
                        L("XepLoaiCode"),
                        L("TinhTrangHonNhanCode"),
                        L("TepDinhKem"),
                        L("DtDiDong"),
                        L("DtCoQuan"),
                        L("DtNhaRieng"),
                        L("DtKhac"),
                        L("EmailCaNhan"),
                        L("EmailCoQuan"),
                        L("EmailKhac"),
                        L("NguyenQuan"),
                        L("TinhThanhID"),
                        L("NoiSinh"),
                        L("Skype"),
                        L("Facebook"),
                        L("QuocGiaHKTT"),
                        L("TinhThanhIDHKTT"),
                        L("DiaChiHKTT"),
                        L("SoSoHoKhau"),
                        L("MaSoHoGiaDinh"),
                        L("LaChuHo"),
                        L("QuocGiaHN"),
                        L("TinhThanhIDHN"),
                        L("DiaChiHN"),
                        L("HoVaTenLHKC"),
                        L("QuanHeLHKC"),
                        L("DtDiDongLHKC"),
                        L("DtNhaRiengLHKC"),
                        L("EmailLHKC"),
                        L("DiaChiLHKC"),
                        L("MaChamCong"),
                        L("ChucDanh"),
                        L("Cap"),
                        L("Bac"),
                        L("TrangThaiLamViecCode"),
                        L("QuanLyTrucTiep"),
                        L("QuanLyGianTiep"),
                        L("DiaDiemLamViecCode"),
                        L("SoSoQLLaoDong"),
                        L("LoaiHopDongCode"),
                        L("NgayTapSu"),
                        L("NgayThuViec"),
                        L("NgayChinhThuc"),
                        L("SoNgayPhep"),
                        L("BacLuongCode"),
                        L("LuongCoBan"),
                        L("LuongDongBH"),
                        L("SoCongChuan"),
                        L("DonViSoCongChuanCode"),
                        L("TkNganHang"),
                        L("NganHangCode"),
                        L("ThamGiaCongDoan"),
                        L("NgayThamGiaBH"),
                        L("TyLeDongBH"),
                        L("SoSoBHXH"),
                        L("MaSoBHXH"),
                        L("MaTinhCap"),
                        L("SoTheBHYT"),
                        L("NgayHetHanBHYT"),
                        L("NoiDangKyKCBID"),
                        L("MaSoNoiKCB"),
                        L("AUTH_STATUS"),
                        L("RECORD_STATUS"),
                        L("MARKER_ID"),
                        L("CHECKER_ID"),
                        L("APPROVE_DT")
                        );

                    AddObjects(
                        sheet, 2, hoSos,
                        _ => _.HoSo.MaNhanVien,
                        _ => _.HoSo.HoVaTen,
                        _ => _.HoSo.AnhDaiDien,
                        _ => _.GioiTinhValue,
                        _ => _timeZoneConverter.Convert(_.HoSo.NgaySinh, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.HoSo.MSTCaNhan,
                        _ => _.DonViCongTacValue,
                        _ => _.HoSo.ViTriCongViecValue,
                        _ => _.HoSo.DanToc,
                        _ => _.HoSo.TonGiao,
                        _ => _.HoSo.QuocTich,
                        _ => _.HoSo.SoCMND,
                        _ => _timeZoneConverter.Convert(_.HoSo.NgayCap, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.HoSo.NoiCap,
                        _ => _timeZoneConverter.Convert(_.HoSo.NgayHetHan, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.HoSo.TrinhDoVanHoa,
                        _ => _.TrinhDoDaoTaoValue,
                        _ => _.NoiDaoTaoValue,
                        _ => _.HoSo.Khoa,
                        _ => _.HoSo.ChuyenNganh,
                        _ => _.HoSo.NamTotNghiep,
                        _ => _.XepLoaiValue,
                        _ => _.TinhTrangHonNhanValue,
                        _ => _.HoSo.TepDinhKem,
                        _ => _.HoSo.DtDiDong,
                        _ => _.HoSo.DtCoQuan,
                        _ => _.HoSo.DtNhaRieng,
                        _ => _.HoSo.DtKhac,
                        _ => _.HoSo.EmailCaNhan,
                        _ => _.HoSo.EmailCoQuan,
                        _ => _.HoSo.EmailKhac,
                        _ => _.HoSo.NguyenQuan,
                        _ => _.TinhThanhIDHKTTValue,
                        _ => _.HoSo.NoiSinh,
                        _ => _.HoSo.Skype,
                        _ => _.HoSo.Facebook,
                        _ => _.HoSo.QuocGiaHKTT,
                        _ => _.TinhThanhIDHKTTValue,
                        _ => _.HoSo.DiaChiHKTT,
                        _ => _.HoSo.SoSoHoKhau,
                        _ => _.HoSo.MaSoHoGiaDinh,
                        _ => _.HoSo.LaChuHo,
                        _ => _.HoSo.QuocGiaHN,
                        _ => _.TinhThanhValue,
                        _ => _.HoSo.DiaChiHN,
                        _ => _.HoSo.HoVaTenLHKC,
                        _ => _.HoSo.QuanHeLHKC,
                        _ => _.HoSo.DtDiDongLHKC,
                        _ => _.HoSo.DtNhaRiengLHKC,
                        _ => _.HoSo.EmailLHKC,
                        _ => _.HoSo.DiaChiLHKC,
                        _ => _.HoSo.MaChamCong,
                        _ => _.HoSo.ChucDanh,
                        _ => _.HoSo.Cap,
                        _ => _.HoSo.Bac,
                        _ => _.HoSo.TrangThaiLamViecCode,
                        _ => _.HoSo.QuanLyTrucTiep,
                        _ => _.HoSo.QuanLyGianTiep,
                        _ => _.HoSo.DiaDiemLamViecCode,
                        _ => _.HoSo.SoSoQLLaoDong,
                        _ => _.LoaiHopDongValue,
                        _ => _timeZoneConverter.Convert(_.HoSo.NgayTapSu, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.HoSo.NgayThuViec, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.HoSo.NgayChinhThuc, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.HoSo.SoNgayPhep,
                        _ => _.HoSo.BacLuongCode,
                        _ => _.HoSo.LuongCoBan,
                        _ => _.HoSo.LuongDongBH,
                        _ => _.HoSo.SoCongChuan,
                        _ => _.HoSo.DonViSoCongChuanCode,
                        _ => _.HoSo.TkNganHang,
                        _ => _.HoSo.NganHangCode,
                        _ => _.HoSo.ThamGiaCongDoan,
                        _ => _timeZoneConverter.Convert(_.HoSo.NgayThamGiaBH, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.HoSo.TyLeDongBH,
                        _ => _.HoSo.SoSoBHXH,
                        _ => _.HoSo.MaSoBHXH,
                        _ => _.HoSo.MaTinhCap,
                        _ => _.HoSo.SoTheBHYT,
                        _ => _timeZoneConverter.Convert(_.HoSo.NgayHetHanBHYT, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.HoSo.NoiDangKyKCBID,
                        _ => _.NoiDangKyValue,
                        _ => _.HoSo.AUTH_STATUS,
                        _ => _.HoSo.RECORD_STATUS,
                        _ => _.HoSo.MARKER_ID,
                        _ => _.HoSo.CHECKER_ID,
                        _ => _timeZoneConverter.Convert(_.HoSo.APPROVE_DT, _abpSession.TenantId, _abpSession.GetUserId())
                        );

					var ngaySinhColumn = sheet.Column(5);
                    ngaySinhColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					ngaySinhColumn.AutoFit();
					var ngayCapColumn = sheet.Column(13);
                    ngayCapColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					ngayCapColumn.AutoFit();
					var ngayHetHanColumn = sheet.Column(15);
                    ngayHetHanColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					ngayHetHanColumn.AutoFit();
					var ngayTapSuColumn = sheet.Column(62);
                    ngayTapSuColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					ngayTapSuColumn.AutoFit();
					var ngayThuViecColumn = sheet.Column(63);
                    ngayThuViecColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					ngayThuViecColumn.AutoFit();
					var ngayChinhThucColumn = sheet.Column(64);
                    ngayChinhThucColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					ngayChinhThucColumn.AutoFit();
					var ngayThamGiaBHColumn = sheet.Column(74);
                    ngayThamGiaBHColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					ngayThamGiaBHColumn.AutoFit();
					var ngayHetHanBHYTColumn = sheet.Column(80);
                    ngayHetHanBHYTColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					ngayHetHanBHYTColumn.AutoFit();
					var approvE_DTColumn = sheet.Column(87);
                    approvE_DTColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					approvE_DTColumn.AutoFit();
					

                });
        }
    }
}
