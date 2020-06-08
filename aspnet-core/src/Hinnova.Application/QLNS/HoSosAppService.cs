

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Hinnova.QLNSExporting;
using Hinnova.QLNSDtos;
using Hinnova.Dto;
using Abp.Application.Services.Dto;
using Hinnova.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Hinnova.Configuration;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using Abp.UI;
using Abp.Organizations;
using GemBox.Spreadsheet;
using System.IO;

namespace Hinnova.QLNS
{
	//[AbpAuthorize(AppPermissions.Pages_HoSos)]
    public class HoSosAppService : HinnovaAppServiceBase, IHoSosAppService
    {
		 private readonly IRepository<HoSo> _hoSoRepository;

		private readonly IWebHostEnvironment _env;
		private readonly IRepository<DangKyKCB> _dangKyKCBRepository;
		private readonly IHoSosExcelExporter _hoSosExcelExporter;
		private readonly IRepository<TruongGiaoDich> _truongGiaoDichRepository;
		private readonly IRepository<TinhThanh> _tinhThanhRepository;
		private readonly IRepository<HopDong> _hopDongRepository;
		private readonly IRepository<NoiDaoTao> _noiDaoTaoRepository;
		private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
		private readonly string connectionString;
		private readonly string mes;

		public HoSosAppService(IRepository<DangKyKCB> dangKyKCBRepository, IWebHostEnvironment env, IWebHostEnvironment hostingEnvironment, IRepository<OrganizationUnit, long> organizationUnitRepository, IRepository<HopDong> hopDongRepository ,IRepository<NoiDaoTao> noiDaoTaoRepository,  IRepository<TinhThanh> tinhThanhRepository, IRepository<TruongGiaoDich> truongGiaoDichRepository , IRepository<HoSo> hoSoRepository, IHoSosExcelExporter hoSosExcelExporter ) 
		  {
			_hoSoRepository = hoSoRepository;
			_env = hostingEnvironment;
			_dangKyKCBRepository = dangKyKCBRepository;
			_hopDongRepository = hopDongRepository;
			_hoSosExcelExporter = hoSosExcelExporter;
			_organizationUnitRepository = organizationUnitRepository;
			_truongGiaoDichRepository = truongGiaoDichRepository;
			_noiDaoTaoRepository = noiDaoTaoRepository;
			_tinhThanhRepository = tinhThanhRepository;
			connectionString = env.GetAppConfiguration().GetConnectionString("Default");
		}


		public bool CheckCMND(string cmnd)
		{
			if (cmnd.IsNullOrEmpty())
				return false;

			var x = _hoSoRepository.GetAll().Where(t => t.IsDeleted == false).Any(k => k.SoCMND == cmnd);
			return x;
		}

		public List<HoSoDto> GetAllCMND()
		{
			return _hoSoRepository.GetAll().Where(t => t.IsDeleted == false).Select(t => new HoSoDto { Id = t.Id, SoCMND = t.SoCMND }).ToList();
		}

		public IQueryable<OrganizationUnit> GetAllCMND1()
		{
			return _organizationUnitRepository.GetAll().Where(t => t.IsDeleted == false).Select(t => new OrganizationUnit { Id = t.Id, DisplayName= t.DisplayName });
		}

		public async Task<List<string>> GetAllUnit()
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				if (conn.State == ConnectionState.Closed)
				{
					await conn.OpenAsync();
				}
				var result = await conn.QueryAsync<string>(sql: "SELECT DisplayName FROM dbo.AbpOrganizationUnits WHERE IsDeleted = 0");
				return result.ToList();
			}
		}

		public async Task<List<CreateOrEditHoSoDto>> GetAllHoSO()
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				if (conn.State == ConnectionState.Closed)
				{
					await conn.OpenAsync();
				}
				var result = await conn.QueryAsync<CreateOrEditHoSoDto>(sql: "SELECT * FROM  HoSo WHERE IsDeleted = 0");
				return result.ToList();
			}
		}

		public async Task<List<CreateOrEditHoSoDto>> GetListNhanVienToCty(string  TenCty)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				if (conn.State == ConnectionState.Closed)
				{
					await conn.OpenAsync();
				}
				var result = await conn.QueryAsync<CreateOrEditHoSoDto>(sql: "SELECT * FROM  HoSo WHERE IsDeleted = 0 and TenCty=N'"+TenCty+"'");
				return result.ToList();
			}
		}

		
		public async Task<List<TruongGiaoDichDto>> GetAllTHHD()
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				if (conn.State == ConnectionState.Closed)
				{
					await conn.OpenAsync();
				}
				var result = await conn.QueryAsync<TruongGiaoDichDto>(sql: "  select * from TruongGiaoDich tg where tg.code= N'THHD'and  tg.isDeleted = 0 ");
				return result.ToList();
			}
		}

		public async Task<PagedResultDto<GetHoSoForViewDto>> GetAll(GetAllHoSosInput input)
         {
			
			var filteredHoSos = _hoSoRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.MaNhanVien.Contains(input.Filter) || e.HoVaTen.Contains(input.Filter) || e.AnhDaiDien.Contains(input.Filter) || e.GioiTinhCode.Contains(input.Filter) || e.MSTCaNhan.Contains(input.Filter) || e.ViTriCongViecCode.Contains(input.Filter) || e.DanToc.Contains(input.Filter) || e.TonGiao.Contains(input.Filter) || e.QuocTich.Contains(input.Filter) || e.SoCMND.Contains(input.Filter) || e.NoiCap.Contains(input.Filter) || e.TrinhDoVanHoa.Contains(input.Filter) || e.TrinhDoDaoTaoCode.Contains(input.Filter) || e.Khoa.Contains(input.Filter) || e.ChuyenNganh.Contains(input.Filter) || e.XepLoaiCode.Contains(input.Filter) || e.TinhTrangHonNhanCode.Contains(input.Filter) || e.TepDinhKem.Contains(input.Filter) || e.DtDiDong.Contains(input.Filter) || e.DtCoQuan.Contains(input.Filter) || e.DtNhaRieng.Contains(input.Filter) || e.DtKhac.Contains(input.Filter) || e.EmailCaNhan.Contains(input.Filter) || e.EmailCoQuan.Contains(input.Filter) || e.EmailKhac.Contains(input.Filter) || e.NguyenQuan.Contains(input.Filter) || e.NoiSinh.Contains(input.Filter) || e.Skype.Contains(input.Filter) || e.Facebook.Contains(input.Filter) || e.QuocGiaHKTT.Contains(input.Filter) || e.DiaChiHKTT.Contains(input.Filter) || e.SoSoHoKhau.Contains(input.Filter) || e.MaSoHoGiaDinh.Contains(input.Filter) || e.QuocGiaHN.Contains(input.Filter) || e.DiaChiHN.Contains(input.Filter) || e.HoVaTenLHKC.Contains(input.Filter) || e.QuanHeLHKC.Contains(input.Filter) || e.DtDiDongLHKC.Contains(input.Filter) || e.DtNhaRiengLHKC.Contains(input.Filter) || e.EmailLHKC.Contains(input.Filter) || e.DiaChiLHKC.Contains(input.Filter) || e.MaChamCong.Contains(input.Filter) || e.ChucDanh.Contains(input.Filter) || e.Cap.Contains(input.Filter) || e.Bac.Contains(input.Filter) || e.TrangThaiLamViecCode.Contains(input.Filter) || e.QuanLyTrucTiep.Contains(input.Filter) || e.QuanLyGianTiep.Contains(input.Filter) || e.DiaDiemLamViecCode.Contains(input.Filter) || e.SoSoQLLaoDong.Contains(input.Filter) || e.BacLuongCode.Contains(input.Filter) || e.DonViSoCongChuanCode.Contains(input.Filter) || e.TkNganHang.Contains(input.Filter) || e.NganHangCode.Contains(input.Filter) || e.SoSoBHXH.Contains(input.Filter) || e.MaSoBHXH.Contains(input.Filter) || e.MaTinhCap.Contains(input.Filter) || e.SoTheBHYT.Contains(input.Filter) || e.MaSoNoiKCB.Contains(input.Filter) || e.AUTH_STATUS.Contains(input.Filter) || e.RECORD_STATUS.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.MaNhanVienFilter),  e => e.MaNhanVien.ToLower() == input.MaNhanVienFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.HoVaTenFilter),  e => e.HoVaTen.ToLower() == input.HoVaTenFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AnhDaiDienFilter),  e => e.AnhDaiDien.ToLower() == input.AnhDaiDienFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.GioiTinhCodeFilter),  e => e.GioiTinhCode.ToLower() == input.GioiTinhCodeFilter.ToLower().Trim())
						.WhereIf(input.MinNgaySinhFilter != null, e => e.NgaySinh >= input.MinNgaySinhFilter)
						.WhereIf(input.MaxNgaySinhFilter != null, e => e.NgaySinh <= input.MaxNgaySinhFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.MSTCaNhanFilter),  e => e.MSTCaNhan.ToLower() == input.MSTCaNhanFilter.ToLower().Trim())
						.WhereIf(input.MinDonViCongTacIDFilter != null, e => e.DonViCongTacID >= input.MinDonViCongTacIDFilter)
						.WhereIf(input.MaxDonViCongTacIDFilter != null, e => e.DonViCongTacID <= input.MaxDonViCongTacIDFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ViTriCongViecCodeFilter),  e => e.ViTriCongViecCode.ToLower() == input.ViTriCongViecCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DanTocFilter),  e => e.DanToc.ToLower() == input.DanTocFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TonGiaoFilter),  e => e.TonGiao.ToLower() == input.TonGiaoFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.QuocTichFilter),  e => e.QuocTich.ToLower() == input.QuocTichFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SoCMNDFilter),  e => e.SoCMND.ToLower() == input.SoCMNDFilter.ToLower().Trim())
						.WhereIf(input.MinNgayCapFilter != null, e => e.NgayCap >= input.MinNgayCapFilter)
						.WhereIf(input.MaxNgayCapFilter != null, e => e.NgayCap <= input.MaxNgayCapFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.NoiCapFilter),  e => e.NoiCap.ToLower() == input.NoiCapFilter.ToLower().Trim())
						.WhereIf(input.MinNgayHetHanFilter != null, e => e.NgayHetHan >= input.MinNgayHetHanFilter)
						.WhereIf(input.MaxNgayHetHanFilter != null, e => e.NgayHetHan <= input.MaxNgayHetHanFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.TrinhDoVanHoaFilter),  e => e.TrinhDoVanHoa.ToLower() == input.TrinhDoVanHoaFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TrinhDoDaoTaoCodeFilter),  e => e.TrinhDoDaoTaoCode.ToLower() == input.TrinhDoDaoTaoCodeFilter.ToLower().Trim())
						.WhereIf(input.MinNoiDaoTaoIDFilter != null, e => e.NoiDaoTaoID >= input.MinNoiDaoTaoIDFilter)
						.WhereIf(input.MaxNoiDaoTaoIDFilter != null, e => e.NoiDaoTaoID <= input.MaxNoiDaoTaoIDFilter)
						//.WhereIf(!string.IsNullOrWhiteSpace(input.NoiDaoTaoCodeFilter),  e => e.NoiDaoTaoCode.ToLower() == input.NoiDaoTaoCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.KhoaFilter),  e => e.Khoa.ToLower() == input.KhoaFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.ChuyenNganhFilter),  e => e.ChuyenNganh.ToLower() == input.ChuyenNganhFilter.ToLower().Trim())
						.WhereIf(input.MinNamTotNghiepFilter != null, e => e.NamTotNghiep >= input.MinNamTotNghiepFilter)
						.WhereIf(input.MaxNamTotNghiepFilter != null, e => e.NamTotNghiep <= input.MaxNamTotNghiepFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.XepLoaiCodeFilter),  e => e.XepLoaiCode.ToLower() == input.XepLoaiCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TinhTrangHonNhanCodeFilter),  e => e.TinhTrangHonNhanCode.ToLower() == input.TinhTrangHonNhanCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TepDinhKemFilter),  e => e.TepDinhKem.ToLower() == input.TepDinhKemFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DtDiDongFilter),  e => e.DtDiDong.ToLower() == input.DtDiDongFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DtCoQuanFilter),  e => e.DtCoQuan.ToLower() == input.DtCoQuanFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DtNhaRiengFilter),  e => e.DtNhaRieng.ToLower() == input.DtNhaRiengFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DtKhacFilter),  e => e.DtKhac.ToLower() == input.DtKhacFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.EmailCaNhanFilter),  e => e.EmailCaNhan.ToLower() == input.EmailCaNhanFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.EmailCoQuanFilter),  e => e.EmailCoQuan.ToLower() == input.EmailCoQuanFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.EmailKhacFilter),  e => e.EmailKhac.ToLower() == input.EmailKhacFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.NguyenQuanFilter),  e => e.NguyenQuan.ToLower() == input.NguyenQuanFilter.ToLower().Trim())
						.WhereIf(input.MinTinhThanhIDFilter != null, e => e.TinhThanhID >= input.MinTinhThanhIDFilter)
						.WhereIf(input.MaxTinhThanhIDFilter != null, e => e.TinhThanhID <= input.MaxTinhThanhIDFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.NoiSinhFilter),  e => e.NoiSinh.ToLower() == input.NoiSinhFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SkypeFilter),  e => e.Skype.ToLower() == input.SkypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.FacebookFilter),  e => e.Facebook.ToLower() == input.FacebookFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.QuocGiaHKTTFilter),  e => e.QuocGiaHKTT.ToLower() == input.QuocGiaHKTTFilter.ToLower().Trim())
						.WhereIf(input.MinTinhThanhIDHKTTFilter != null, e => e.TinhThanhIDHKTT >= input.MinTinhThanhIDHKTTFilter)
						.WhereIf(input.MaxTinhThanhIDHKTTFilter != null, e => e.TinhThanhIDHKTT <= input.MaxTinhThanhIDHKTTFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DiaChiHKTTFilter),  e => e.DiaChiHKTT.ToLower() == input.DiaChiHKTTFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SoSoHoKhauFilter),  e => e.SoSoHoKhau.ToLower() == input.SoSoHoKhauFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.MaSoHoGiaDinhFilter),  e => e.MaSoHoGiaDinh.ToLower() == input.MaSoHoGiaDinhFilter.ToLower().Trim())
						 .WhereIf(input.LaChuHoFilter > -1, e => (input.LaChuHoFilter == 1 && e.LaChuHo) || (input.LaChuHoFilter == 0 && !e.LaChuHo))
						//.WhereIf(input.LaChuHoFilter > -1,  e => Convert.ToInt32(e.LaChuHo) == input.LaChuHoFilter )
						.WhereIf(!string.IsNullOrWhiteSpace(input.QuocGiaHNFilter),  e => e.QuocGiaHN.ToLower() == input.QuocGiaHNFilter.ToLower().Trim())
						.WhereIf(input.MinTinhThanhIDHNFilter != null, e => e.TinhThanhIDHN >= input.MinTinhThanhIDHNFilter)
						.WhereIf(input.MaxTinhThanhIDHNFilter != null, e => e.TinhThanhIDHN <= input.MaxTinhThanhIDHNFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DiaChiHNFilter),  e => e.DiaChiHN.ToLower() == input.DiaChiHNFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.HoVaTenLHKCFilter),  e => e.HoVaTenLHKC.ToLower() == input.HoVaTenLHKCFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.QuanHeLHKCFilter),  e => e.QuanHeLHKC.ToLower() == input.QuanHeLHKCFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DtDiDongLHKCFilter),  e => e.DtDiDongLHKC.ToLower() == input.DtDiDongLHKCFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DtNhaRiengLHKCFilter),  e => e.DtNhaRiengLHKC.ToLower() == input.DtNhaRiengLHKCFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.EmailLHKCFilter),  e => e.EmailLHKC.ToLower() == input.EmailLHKCFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DiaChiLHKCFilter),  e => e.DiaChiLHKC.ToLower() == input.DiaChiLHKCFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.MaChamCongFilter),  e => e.MaChamCong.ToLower() == input.MaChamCongFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.ChucDanhFilter),  e => e.ChucDanh.ToLower() == input.ChucDanhFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CapFilter),  e => e.Cap.ToLower() == input.CapFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.BacFilter),  e => e.Bac.ToLower() == input.BacFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TrangThaiLamViecCodeFilter),  e => e.TrangThaiLamViecCode.ToLower() == input.TrangThaiLamViecCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.QuanLyTrucTiepFilter),  e => e.QuanLyTrucTiep.ToLower() == input.QuanLyTrucTiepFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.QuanLyGianTiepFilter),  e => e.QuanLyGianTiep.ToLower() == input.QuanLyGianTiepFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DiaDiemLamViecCodeFilter),  e => e.DiaDiemLamViecCode.ToLower() == input.DiaDiemLamViecCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SoSoQLLaoDongFilter),  e => e.SoSoQLLaoDong.ToLower() == input.SoSoQLLaoDongFilter.ToLower().Trim())
						.WhereIf(input.MinLoaiHopDongIDFilter != null, e => e.LoaiHopDongID >= input.MinLoaiHopDongIDFilter)
						.WhereIf(input.MaxLoaiHopDongIDFilter != null, e => e.LoaiHopDongID <= input.MaxLoaiHopDongIDFilter)
						
						//.WhereIf(!string.IsNullOrWhiteSpace(input.LoaiHopDongCodeFilter),  e => e.LoaiHopDongCode.ToLower() == input.LoaiHopDongCodeFilter.ToLower().Trim())
						.WhereIf(input.MinNgayTapSuFilter != null, e => e.NgayTapSu >= input.MinNgayTapSuFilter)
						.WhereIf(input.MaxNgayTapSuFilter != null, e => e.NgayTapSu <= input.MaxNgayTapSuFilter)
						.WhereIf(input.MinNgayThuViecFilter != null, e => e.NgayThuViec >= input.MinNgayThuViecFilter)
						.WhereIf(input.MaxNgayThuViecFilter != null, e => e.NgayThuViec <= input.MaxNgayThuViecFilter)
						.WhereIf(input.MinNgayChinhThucFilter != null, e => e.NgayChinhThuc >= input.MinNgayChinhThucFilter)
						.WhereIf(input.MaxNgayChinhThucFilter != null, e => e.NgayChinhThuc <= input.MaxNgayChinhThucFilter)
						.WhereIf(input.MinSoNgayPhepFilter != null, e => e.SoNgayPhep >= input.MinSoNgayPhepFilter)
						.WhereIf(input.MaxSoNgayPhepFilter != null, e => e.SoNgayPhep <= input.MaxSoNgayPhepFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.BacLuongCodeFilter),  e => e.BacLuongCode.ToLower() == input.BacLuongCodeFilter.ToLower().Trim())
						.WhereIf(input.MinLuongCoBanFilter != null, e => e.LuongCoBan >= input.MinLuongCoBanFilter)
						.WhereIf(input.MaxLuongCoBanFilter != null, e => e.LuongCoBan <= input.MaxLuongCoBanFilter)
						.WhereIf(input.MinLuongDongBHFilter != null, e => e.LuongDongBH >= input.MinLuongDongBHFilter)
						.WhereIf(input.MaxLuongDongBHFilter != null, e => e.LuongDongBH <= input.MaxLuongDongBHFilter)
						.WhereIf(input.MinSoCongChuanFilter != null, e => e.SoCongChuan >= input.MinSoCongChuanFilter)
						.WhereIf(input.MaxSoCongChuanFilter != null, e => e.SoCongChuan <= input.MaxSoCongChuanFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DonViSoCongChuanCodeFilter),  e => e.DonViSoCongChuanCode.ToLower() == input.DonViSoCongChuanCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TkNganHangFilter),  e => e.TkNganHang.ToLower() == input.TkNganHangFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.NganHangCodeFilter),  e => e.NganHangCode.ToLower() == input.NganHangCodeFilter.ToLower().Trim())
					   .WhereIf(input.ThamGiaCongDoanFilter > -1, e => (input.ThamGiaCongDoanFilter == 1 && e.ThamGiaCongDoan) || (input.ThamGiaCongDoanFilter == 0 && !e.ThamGiaCongDoan))
						//.WhereIf(input.ThamGiaCongDoanFilter > -1,  e => Convert.ToInt32(e.ThamGiaCongDoan) == input.ThamGiaCongDoanFilter )
						.WhereIf(input.MinNgayThamGiaBHFilter != null, e => e.NgayThamGiaBH >= input.MinNgayThamGiaBHFilter)
						.WhereIf(input.MaxNgayThamGiaBHFilter != null, e => e.NgayThamGiaBH <= input.MaxNgayThamGiaBHFilter)
						.WhereIf(input.MinTyLeDongBHFilter != null, e => e.TyLeDongBH >= input.MinTyLeDongBHFilter)
						.WhereIf(input.MaxTyLeDongBHFilter != null, e => e.TyLeDongBH <= input.MaxTyLeDongBHFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.SoSoBHXHFilter),  e => e.SoSoBHXH.ToLower() == input.SoSoBHXHFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.MaSoBHXHFilter),  e => e.MaSoBHXH.ToLower() == input.MaSoBHXHFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.MaTinhCapFilter),  e => e.MaTinhCap.ToLower() == input.MaTinhCapFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SoTheBHYTFilter),  e => e.SoTheBHYT.ToLower() == input.SoTheBHYTFilter.ToLower().Trim())
						.WhereIf(input.MinNgayHetHanBHYTFilter != null, e => e.NgayHetHanBHYT >= input.MinNgayHetHanBHYTFilter)
						.WhereIf(input.MaxNgayHetHanBHYTFilter != null, e => e.NgayHetHanBHYT <= input.MaxNgayHetHanBHYTFilter)
						.WhereIf(input.MinNoiDangKyKCBIDFilter != null, e => e.NoiDangKyKCBID >= input.MinNoiDangKyKCBIDFilter)
						.WhereIf(input.MaxNoiDangKyKCBIDFilter != null, e => e.NoiDangKyKCBID <= input.MaxNoiDangKyKCBIDFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.MaSoNoiKCBFilter),  e => e.MaSoNoiKCB.ToLower() == input.MaSoNoiKCBFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AUTH_STATUSFilter),  e => e.AUTH_STATUS.ToLower() == input.AUTH_STATUSFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.RECORD_STATUSFilter),  e => e.RECORD_STATUS.ToLower() == input.RECORD_STATUSFilter.ToLower().Trim())
						.WhereIf(input.MinMARKER_IDFilter != null, e => e.MARKER_ID >= input.MinMARKER_IDFilter)
						.WhereIf(input.MaxMARKER_IDFilter != null, e => e.MARKER_ID <= input.MaxMARKER_IDFilter)
						.WhereIf(input.MinCHECKER_IDFilter != null, e => e.CHECKER_ID >= input.MinCHECKER_IDFilter)
						.WhereIf(input.MaxCHECKER_IDFilter != null, e => e.CHECKER_ID <= input.MaxCHECKER_IDFilter)
						.WhereIf(input.MinAPPROVE_DTFilter != null, e => e.APPROVE_DT >= input.MinAPPROVE_DTFilter)
						.WhereIf(input.MaxAPPROVE_DTFilter != null, e => e.APPROVE_DT <= input.MaxAPPROVE_DTFilter);
			var pagedAndFilteredHoSos = filteredHoSos
				.OrderBy(input.Sorting ?? "id asc")
				.PageBy(input);

					var tgd = _truongGiaoDichRepository.GetAll();

			         var units = _organizationUnitRepository.GetAll();
			        
			var hoSos = from o in pagedAndFilteredHoSos
						join vtut in tgd.Where(x => x.Code == "VTUT")
						   on o.ViTriCongViecCode equals vtut.CDName into vtutJoin
						from joinedvtut in vtutJoin.DefaultIfEmpty()

						join gt in tgd.Where(x => x.Code == "GT") on o.GioiTinhCode equals gt.CDName into gtJoin
						from joinedgt in gtJoin.DefaultIfEmpty()

						join tthn in tgd.Where(x => x.Code == "TTHN") on o.TinhTrangHonNhanCode equals tthn.CDName into tthnJoin
						from joinedtthn in tthnJoin.DefaultIfEmpty()

						join tddt in tgd.Where(x => x.Code == "TDDT") on o.TrinhDoDaoTaoCode equals tddt.CDName into tddtJoin
						from joinedtddt in tddtJoin.DefaultIfEmpty()

						join xl in tgd.Where(x => x.Code == "XLHL") on o.XepLoaiCode equals xl.CDName into xlJoin
						from joinedxl in xlJoin.DefaultIfEmpty()

						join lhd in tgd.Where(x => x.Code == "LHDG") on o.LoaiHopDongID equals lhd.Id into lhdJoin
						from joinelhd in lhdJoin.DefaultIfEmpty()

						join ndt in _noiDaoTaoRepository.GetAll() on o.NoiDaoTaoID equals ndt.Id into ndtJoin
						from joinedndt in ndtJoin.DefaultIfEmpty()

						join tinhThanh1 in _tinhThanhRepository.GetAll() on o.TinhThanhIDHKTT equals tinhThanh1.Id into tinhThanh1Join
						from joinedtinhThanh1 in tinhThanh1Join.DefaultIfEmpty()

						join tinhThanh2 in _tinhThanhRepository.GetAll() on o.TinhThanhID equals tinhThanh2.Id into tinhThanh2Join
						from joinedtinhThanh2 in tinhThanh2Join.DefaultIfEmpty()

						join tinhThanh3 in _tinhThanhRepository.GetAll() on o.TinhThanhIDHN equals tinhThanh3.Id into tinhThanh3Join
						from joinedtinhThanh3 in tinhThanh3Join.DefaultIfEmpty()
							//code loại hợp đồng đâu
						join hopdong in _hopDongRepository.GetAll() on o.LoaiHopDongID equals hopdong.Id into hopdongjoin
						from joinedthopdong in hopdongjoin.DefaultIfEmpty()

					

						join unit in units on o.DonViCongTacID.Value equals unit.Id into unitjoin
						from joinedtunit in unitjoin.DefaultIfEmpty()

						join noiDangKy in _dangKyKCBRepository.GetAll() on o.NoiDangKyKCBID.Value equals noiDangKy.Id into noiDangKyjoin
						from joinedtnoiDangKy in noiDangKyjoin.DefaultIfEmpty()


						select new GetHoSoForViewDto() {
							HoSo = new HoSoDto
							{
                                MaNhanVien = o.MaNhanVien,
                                HoVaTen = o.HoVaTen,
                                AnhDaiDien = o.AnhDaiDien,
                                GioiTinhValue = o.GioiTinhCode,
                                NgaySinh = o.NgaySinh,
                                MSTCaNhan = o.MSTCaNhan,
                                DonViCongTacID = o.DonViCongTacID,
                                ViTriCongViecValue = o.ViTriCongViecCode,
                                DanToc = o.DanToc,
                                TonGiao = o.TonGiao,
                                QuocTich = o.QuocTich,
                                SoCMND = o.SoCMND,
                                NgayCap = o.NgayCap,
                                NoiCap = o.NoiCap,
                                NgayHetHan = o.NgayHetHan,
                                TrinhDoVanHoa = o.TrinhDoVanHoa,
                                TrinhDoDaoTaoCode = o.TrinhDoDaoTaoCode,
                                NoiDaoTaoID = o.NoiDaoTaoID,
                                Khoa = o.Khoa,
                                ChuyenNganh = o.ChuyenNganh,
                                NamTotNghiep = o.NamTotNghiep,
                                XepLoaiCode = o.XepLoaiCode,
                                TinhTrangHonNhanCode = o.TinhTrangHonNhanCode,
                                TepDinhKem = o.TepDinhKem,
                                DtDiDong = o.DtDiDong,
                                DtCoQuan = o.DtCoQuan,
                                DtNhaRieng = o.DtNhaRieng,
                                DtKhac = o.DtKhac,
                                EmailCaNhan = o.EmailCaNhan,
                                EmailCoQuan = o.EmailCoQuan,
                                EmailKhac = o.EmailKhac,
                                NguyenQuan = o.NguyenQuan,
                                TinhThanhID = o.TinhThanhID,
                                NoiSinh = o.NoiSinh,
                                Skype = o.Skype,
                                Facebook = o.Facebook,
                                QuocGiaHKTT = o.QuocGiaHKTT,
                                TinhThanhIDHKTT = o.TinhThanhIDHKTT,
                                DiaChiHKTT = o.DiaChiHKTT,
                                SoSoHoKhau = o.SoSoHoKhau,
                                MaSoHoGiaDinh = o.MaSoHoGiaDinh,
                                LaChuHo = o.LaChuHo,
                                QuocGiaHN = o.QuocGiaHN,
                                TinhThanhIDHN = o.TinhThanhIDHN,
                                DiaChiHN = o.DiaChiHN,
                                HoVaTenLHKC = o.HoVaTenLHKC,
                                QuanHeLHKC = o.QuanHeLHKC,
                                DtDiDongLHKC = o.DtDiDongLHKC,
                                DtNhaRiengLHKC = o.DtNhaRiengLHKC,
                                EmailLHKC = o.EmailLHKC,
                                DiaChiLHKC = o.DiaChiLHKC,
                                MaChamCong = o.MaChamCong,
                                ChucDanh = o.ChucDanh,
                                Cap = o.Cap,
                                Bac = o.Bac,
                                TrangThaiLamViecCode = o.TrangThaiLamViecCode,
                                QuanLyTrucTiep = o.QuanLyTrucTiep,
                                QuanLyGianTiep = o.QuanLyGianTiep,
                                DiaDiemLamViecCode = o.DiaDiemLamViecCode,
                                SoSoQLLaoDong = o.SoSoQLLaoDong,
                                HopDongValue = o.LoaiHopDongID,
                                NgayTapSu = o.NgayTapSu,
                                NgayThuViec = o.NgayThuViec,
                                NgayChinhThuc = o.NgayChinhThuc,
                                SoNgayPhep = o.SoNgayPhep,
                                BacLuongCode = o.BacLuongCode,
                                LuongCoBan = o.LuongCoBan,
                                LuongDongBH = o.LuongDongBH,
                                SoCongChuan = o.SoCongChuan,
                                DonViSoCongChuanCode = o.DonViSoCongChuanCode,
                                TkNganHang = o.TkNganHang,
                                NganHangCode = o.NganHangCode,
                                ThamGiaCongDoan = o.ThamGiaCongDoan,
                                NgayThamGiaBH = o.NgayThamGiaBH,
                                TyLeDongBH = o.TyLeDongBH,
                                SoSoBHXH = o.SoSoBHXH,
                                MaSoBHXH = o.MaSoBHXH,
                                MaTinhCap = o.MaTinhCap,
                                SoTheBHYT = o.SoTheBHYT,
                                NgayHetHanBHYT = o.NgayHetHanBHYT,
                                NoiDangKyKCBID = o.NoiDangKyKCBID,
                                MaSoNoiKCB = o.MaSoNoiKCB,
                                AUTH_STATUS = o.AUTH_STATUS,
                                RECORD_STATUS = o.RECORD_STATUS,
                                MARKER_ID = o.MARKER_ID,
                                CHECKER_ID = o.CHECKER_ID,
                                APPROVE_DT = o.APPROVE_DT,
                                Id = o.Id
							},
							LoaiHopDongValue = joinelhd ==null ? "" : joinelhd.Value.ToString(),
							ViTriCongViecValue = joinedvtut == null ? "" : joinedvtut.Value.ToString(),
							GioiTinhValue = joinedgt == null ? "" : joinedgt.Value.ToString(),
							TinhTrangHonNhanValue = joinedtthn == null ? "" : joinedtthn.Value.ToString(),
							TrinhDoDaoTaoValue = joinedtddt == null ? "" : joinedtddt.Value.ToString(),
							XepLoaiValue = joinedxl == null ? "" : joinedxl.Value.ToString(),
							NoiDaoTaoValue = joinedndt == null ? "" : joinedndt.TenNoiDaoTao.ToString(),
							TinhThanhIDHKTTValue = joinedtinhThanh1 == null ? "" : joinedtinhThanh1.TenTinhThanh.ToString(),
							TinhThanhValue = joinedtinhThanh2 == null ? "" : joinedtinhThanh2.TenTinhThanh.ToString(),
							TinhThanhIDHNValue = joinedtinhThanh3 == null ? "" : joinedtinhThanh3.TenTinhThanh.ToString(),
							HopDongValue = joinedthopdong == null ? "" : joinedthopdong.TenHopDong.ToString(),
							DonViCongTacValue = joinedtunit == null ? "" : joinedtunit.DisplayName.ToString(),
							NoiDangKyValue = joinedtnoiDangKy == null ? "" : joinedtnoiDangKy.TenNoiKCB.ToString(),
						};
			
			 var totalCount = await filteredHoSos.CountAsync();

            return new PagedResultDto<GetHoSoForViewDto>(
                totalCount,
                await hoSos.ToListAsync()
            );
         }
		//public List<HoSoDto> GetAllHoSo()
		//{
		//	return ObjectMapper.Map<List<HoSoDto>>(_hoSoRepository.GetAll().ToList());
		//}
		
		public async Task<List<string>> GetNameUnit(int id)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				if (conn.State == ConnectionState.Closed)

				{
					await conn.OpenAsync();
				}
				var result = await conn.QueryAsync<string>(sql: "SELECT DisplayName FROM dbo.AbpOrganizationUnits WHERE IsDeleted = 0 and Id="+id);
				return result.ToList();
			}
		}
		

		public async Task<List<ListInfoHoSo>> GetListInFoHoSo()
		{

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				if (conn.State == ConnectionState.Closed)
				{
					await conn.OpenAsync();
				}
				var result = await conn.QueryAsync<ListInfoHoSo>(sql: "dbo.GetListInfoHoSo", param: new { }, commandType: CommandType.StoredProcedure);
				return result.ToList();
			}
		}

		public async Task<List<CreateOrEditHoSoDto>> GetListNhanVienToDataFromDate(string fromDate, string toDate ,string tenCty)
		{

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				if (conn.State == ConnectionState.Closed)
				{
					await conn.OpenAsync();
				}
				var result = await conn.QueryAsync<CreateOrEditHoSoDto>(sql: "dbo.GetListNhanVienToDateFromDate", param: new { fromDate , toDate, tenCty }, commandType: CommandType.StoredProcedure);
				return result.ToList();
			}
		}
		public async Task<CreateOrEditHoSoDto> GetHoSoForView(int id)
         {
			var hoso = await _hoSoRepository.GetAsync(id);
			return ObjectMapper.Map<CreateOrEditHoSoDto>(hoso);
			//using (SqlConnection conn = new SqlConnection(connectionString))
			//{
			//	if (conn.State == ConnectionState.Closed)
			//	{
			//		await conn.OpenAsync();
			//	}
			//	var result = await conn.QueryAsync<CreateOrEditHoSoDto>(sql: "SELECT * FROM HoSo WHERE IsDeleted = 0 and Id=" + id);
			//	return result;
			//}
		}
		 
		 //[AbpAuthorize(AppPermissions.Pages_HoSos_Edit)]
		 public async Task<GetHoSoForEditOutput> GetHoSoForEdit(EntityDto input)
         {
            var hoSo = await _hoSoRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetHoSoForEditOutput {HoSo = ObjectMapper.Map<CreateOrEditHoSoDto>(hoSo)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditHoSoDto input)
         {
            if(input.Id == null){
				//if (CheckCMND(input.SoCMND))
				//{
				//	throw new UserFriendlyException("Số CMND đã bị trùng");
				//}
				
				using (SqlConnection conn = new SqlConnection(connectionString))
				{
					if (conn.State == ConnectionState.Closed)
						await conn.OpenAsync();
					var tableName = "HoSo";
					var result = await conn.QueryAsync<string>(sql: "exec SYS_CodeMasters_Gen " + tableName);
					input.MaNhanVien = result.ToList().First();
					await Create(input);
				}
			
			}
			else{
				await Update(input);
			}
         }

		 //[AbpAuthorize(AppPermissions.Pages_HoSos_Create)]
		 protected virtual async Task Create(CreateOrEditHoSoDto input)
         {
            var hoSo = ObjectMapper.Map<HoSo>(input);

			

            await _hoSoRepository.InsertAsync(hoSo);
         }

		 //[AbpAuthorize(AppPermissions.Pages_HoSos_Edit)]
		 protected virtual async Task Update(CreateOrEditHoSoDto input)
         {
            var hoSo = await _hoSoRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, hoSo);
         }

		 //[AbpAuthorize(AppPermissions.Pages_HoSos_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _hoSoRepository.DeleteAsync(input.Id);
         }

		public async Task<FileDto> GetHoSosToExcel(GetAllHoSosForExcelInput input)
		{

			var filteredHoSos = _hoSoRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.MaNhanVien.Contains(input.Filter) || e.HoVaTen.Contains(input.Filter) || e.AnhDaiDien.Contains(input.Filter) || e.GioiTinhCode.Contains(input.Filter) || e.MSTCaNhan.Contains(input.Filter) || e.ViTriCongViecCode.Contains(input.Filter) || e.DanToc.Contains(input.Filter) || e.TonGiao.Contains(input.Filter) || e.QuocTich.Contains(input.Filter) || e.SoCMND.Contains(input.Filter) || e.NoiCap.Contains(input.Filter) || e.TrinhDoVanHoa.Contains(input.Filter) || e.TrinhDoDaoTaoCode.Contains(input.Filter) || e.Khoa.Contains(input.Filter) || e.ChuyenNganh.Contains(input.Filter) || e.XepLoaiCode.Contains(input.Filter) || e.TinhTrangHonNhanCode.Contains(input.Filter) || e.TepDinhKem.Contains(input.Filter) || e.DtDiDong.Contains(input.Filter) || e.DtCoQuan.Contains(input.Filter) || e.DtNhaRieng.Contains(input.Filter) || e.DtKhac.Contains(input.Filter) || e.EmailCaNhan.Contains(input.Filter) || e.EmailCoQuan.Contains(input.Filter) || e.EmailKhac.Contains(input.Filter) || e.NguyenQuan.Contains(input.Filter) || e.NoiSinh.Contains(input.Filter) || e.Skype.Contains(input.Filter) || e.Facebook.Contains(input.Filter) || e.QuocGiaHKTT.Contains(input.Filter) || e.DiaChiHKTT.Contains(input.Filter) || e.SoSoHoKhau.Contains(input.Filter) || e.MaSoHoGiaDinh.Contains(input.Filter) || e.QuocGiaHN.Contains(input.Filter) || e.DiaChiHN.Contains(input.Filter) || e.HoVaTenLHKC.Contains(input.Filter) || e.QuanHeLHKC.Contains(input.Filter) || e.DtDiDongLHKC.Contains(input.Filter) || e.DtNhaRiengLHKC.Contains(input.Filter) || e.EmailLHKC.Contains(input.Filter) || e.DiaChiLHKC.Contains(input.Filter) || e.MaChamCong.Contains(input.Filter) || e.ChucDanh.Contains(input.Filter) || e.Cap.Contains(input.Filter) || e.Bac.Contains(input.Filter) || e.TrangThaiLamViecCode.Contains(input.Filter) || e.QuanLyTrucTiep.Contains(input.Filter) || e.QuanLyGianTiep.Contains(input.Filter) || e.DiaDiemLamViecCode.Contains(input.Filter) || e.SoSoQLLaoDong.Contains(input.Filter) || e.BacLuongCode.Contains(input.Filter) || e.DonViSoCongChuanCode.Contains(input.Filter) || e.TkNganHang.Contains(input.Filter) || e.NganHangCode.Contains(input.Filter) || e.SoSoBHXH.Contains(input.Filter) || e.MaSoBHXH.Contains(input.Filter) || e.MaTinhCap.Contains(input.Filter) || e.SoTheBHYT.Contains(input.Filter) || e.MaSoNoiKCB.Contains(input.Filter) || e.AUTH_STATUS.Contains(input.Filter) || e.RECORD_STATUS.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.MaNhanVienFilter), e => e.MaNhanVien.ToLower() == input.MaNhanVienFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.HoVaTenFilter), e => e.HoVaTen.ToLower() == input.HoVaTenFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AnhDaiDienFilter), e => e.AnhDaiDien.ToLower() == input.AnhDaiDienFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.GioiTinhCodeFilter), e => e.GioiTinhCode.ToLower() == input.GioiTinhCodeFilter.ToLower().Trim())
						.WhereIf(input.MinNgaySinhFilter != null, e => e.NgaySinh >= input.MinNgaySinhFilter)
						.WhereIf(input.MaxNgaySinhFilter != null, e => e.NgaySinh <= input.MaxNgaySinhFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.MSTCaNhanFilter), e => e.MSTCaNhan.ToLower() == input.MSTCaNhanFilter.ToLower().Trim())
						.WhereIf(input.MinDonViCongTacIDFilter != null, e => e.DonViCongTacID >= input.MinDonViCongTacIDFilter)
						.WhereIf(input.MaxDonViCongTacIDFilter != null, e => e.DonViCongTacID <= input.MaxDonViCongTacIDFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ViTriCongViecCodeFilter), e => e.ViTriCongViecCode.ToLower() == input.ViTriCongViecCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DanTocFilter), e => e.DanToc.ToLower() == input.DanTocFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TonGiaoFilter), e => e.TonGiao.ToLower() == input.TonGiaoFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.QuocTichFilter), e => e.QuocTich.ToLower() == input.QuocTichFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SoCMNDFilter), e => e.SoCMND.ToLower() == input.SoCMNDFilter.ToLower().Trim())
						.WhereIf(input.MinNgayCapFilter != null, e => e.NgayCap >= input.MinNgayCapFilter)
						.WhereIf(input.MaxNgayCapFilter != null, e => e.NgayCap <= input.MaxNgayCapFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.NoiCapFilter), e => e.NoiCap.ToLower() == input.NoiCapFilter.ToLower().Trim())
						.WhereIf(input.MinNgayHetHanFilter != null, e => e.NgayHetHan >= input.MinNgayHetHanFilter)
						.WhereIf(input.MaxNgayHetHanFilter != null, e => e.NgayHetHan <= input.MaxNgayHetHanFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.TrinhDoVanHoaFilter), e => e.TrinhDoVanHoa.ToLower() == input.TrinhDoVanHoaFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TrinhDoDaoTaoCodeFilter), e => e.TrinhDoDaoTaoCode.ToLower() == input.TrinhDoDaoTaoCodeFilter.ToLower().Trim())
						.WhereIf(input.MinNoiDaoTaoIDFilter != null, e => e.NoiDaoTaoID >= input.MinNoiDaoTaoIDFilter)
						.WhereIf(input.MaxNoiDaoTaoIDFilter != null, e => e.NoiDaoTaoID <= input.MaxNoiDaoTaoIDFilter)
						//.WhereIf(!string.IsNullOrWhiteSpace(input.NoiDaoTaoCodeFilter),  e => e.NoiDaoTaoCode.ToLower() == input.NoiDaoTaoCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.KhoaFilter), e => e.Khoa.ToLower() == input.KhoaFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.ChuyenNganhFilter), e => e.ChuyenNganh.ToLower() == input.ChuyenNganhFilter.ToLower().Trim())
						.WhereIf(input.MinNamTotNghiepFilter != null, e => e.NamTotNghiep >= input.MinNamTotNghiepFilter)
						.WhereIf(input.MaxNamTotNghiepFilter != null, e => e.NamTotNghiep <= input.MaxNamTotNghiepFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.XepLoaiCodeFilter), e => e.XepLoaiCode.ToLower() == input.XepLoaiCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TinhTrangHonNhanCodeFilter), e => e.TinhTrangHonNhanCode.ToLower() == input.TinhTrangHonNhanCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TepDinhKemFilter), e => e.TepDinhKem.ToLower() == input.TepDinhKemFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DtDiDongFilter), e => e.DtDiDong.ToLower() == input.DtDiDongFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DtCoQuanFilter), e => e.DtCoQuan.ToLower() == input.DtCoQuanFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DtNhaRiengFilter), e => e.DtNhaRieng.ToLower() == input.DtNhaRiengFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DtKhacFilter), e => e.DtKhac.ToLower() == input.DtKhacFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.EmailCaNhanFilter), e => e.EmailCaNhan.ToLower() == input.EmailCaNhanFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.EmailCoQuanFilter), e => e.EmailCoQuan.ToLower() == input.EmailCoQuanFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.EmailKhacFilter), e => e.EmailKhac.ToLower() == input.EmailKhacFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.NguyenQuanFilter), e => e.NguyenQuan.ToLower() == input.NguyenQuanFilter.ToLower().Trim())
						.WhereIf(input.MinTinhThanhIDFilter != null, e => e.TinhThanhID >= input.MinTinhThanhIDFilter)
						.WhereIf(input.MaxTinhThanhIDFilter != null, e => e.TinhThanhID <= input.MaxTinhThanhIDFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.NoiSinhFilter), e => e.NoiSinh.ToLower() == input.NoiSinhFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SkypeFilter), e => e.Skype.ToLower() == input.SkypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.FacebookFilter), e => e.Facebook.ToLower() == input.FacebookFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.QuocGiaHKTTFilter), e => e.QuocGiaHKTT.ToLower() == input.QuocGiaHKTTFilter.ToLower().Trim())
						.WhereIf(input.MinTinhThanhIDHKTTFilter != null, e => e.TinhThanhIDHKTT >= input.MinTinhThanhIDHKTTFilter)
						.WhereIf(input.MaxTinhThanhIDHKTTFilter != null, e => e.TinhThanhIDHKTT <= input.MaxTinhThanhIDHKTTFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DiaChiHKTTFilter), e => e.DiaChiHKTT.ToLower() == input.DiaChiHKTTFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SoSoHoKhauFilter), e => e.SoSoHoKhau.ToLower() == input.SoSoHoKhauFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.MaSoHoGiaDinhFilter), e => e.MaSoHoGiaDinh.ToLower() == input.MaSoHoGiaDinhFilter.ToLower().Trim())
						 .WhereIf(input.LaChuHoFilter > -1, e => (input.LaChuHoFilter == 1 && e.LaChuHo) || (input.LaChuHoFilter == 0 && !e.LaChuHo))
						//.WhereIf(input.LaChuHoFilter > -1,  e => Convert.ToInt32(e.LaChuHo) == input.LaChuHoFilter )
						.WhereIf(!string.IsNullOrWhiteSpace(input.QuocGiaHNFilter), e => e.QuocGiaHN.ToLower() == input.QuocGiaHNFilter.ToLower().Trim())
						.WhereIf(input.MinTinhThanhIDHNFilter != null, e => e.TinhThanhIDHN >= input.MinTinhThanhIDHNFilter)
						.WhereIf(input.MaxTinhThanhIDHNFilter != null, e => e.TinhThanhIDHN <= input.MaxTinhThanhIDHNFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DiaChiHNFilter), e => e.DiaChiHN.ToLower() == input.DiaChiHNFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.HoVaTenLHKCFilter), e => e.HoVaTenLHKC.ToLower() == input.HoVaTenLHKCFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.QuanHeLHKCFilter), e => e.QuanHeLHKC.ToLower() == input.QuanHeLHKCFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DtDiDongLHKCFilter), e => e.DtDiDongLHKC.ToLower() == input.DtDiDongLHKCFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DtNhaRiengLHKCFilter), e => e.DtNhaRiengLHKC.ToLower() == input.DtNhaRiengLHKCFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.EmailLHKCFilter), e => e.EmailLHKC.ToLower() == input.EmailLHKCFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DiaChiLHKCFilter), e => e.DiaChiLHKC.ToLower() == input.DiaChiLHKCFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.MaChamCongFilter), e => e.MaChamCong.ToLower() == input.MaChamCongFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.ChucDanhFilter), e => e.ChucDanh.ToLower() == input.ChucDanhFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CapFilter), e => e.Cap.ToLower() == input.CapFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.BacFilter), e => e.Bac.ToLower() == input.BacFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TrangThaiLamViecCodeFilter), e => e.TrangThaiLamViecCode.ToLower() == input.TrangThaiLamViecCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.QuanLyTrucTiepFilter), e => e.QuanLyTrucTiep.ToLower() == input.QuanLyTrucTiepFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.QuanLyGianTiepFilter), e => e.QuanLyGianTiep.ToLower() == input.QuanLyGianTiepFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DiaDiemLamViecCodeFilter), e => e.DiaDiemLamViecCode.ToLower() == input.DiaDiemLamViecCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SoSoQLLaoDongFilter), e => e.SoSoQLLaoDong.ToLower() == input.SoSoQLLaoDongFilter.ToLower().Trim())
						.WhereIf(input.MinLoaiHopDongIDFilter != null, e => e.LoaiHopDongID >= input.MinLoaiHopDongIDFilter)
						.WhereIf(input.MaxLoaiHopDongIDFilter != null, e => e.LoaiHopDongID <= input.MaxLoaiHopDongIDFilter)

						//.WhereIf(!string.IsNullOrWhiteSpace(input.LoaiHopDongCodeFilter),  e => e.LoaiHopDongCode.ToLower() == input.LoaiHopDongCodeFilter.ToLower().Trim())
						.WhereIf(input.MinNgayTapSuFilter != null, e => e.NgayTapSu >= input.MinNgayTapSuFilter)
						.WhereIf(input.MaxNgayTapSuFilter != null, e => e.NgayTapSu <= input.MaxNgayTapSuFilter)
						.WhereIf(input.MinNgayThuViecFilter != null, e => e.NgayThuViec >= input.MinNgayThuViecFilter)
						.WhereIf(input.MaxNgayThuViecFilter != null, e => e.NgayThuViec <= input.MaxNgayThuViecFilter)
						.WhereIf(input.MinNgayChinhThucFilter != null, e => e.NgayChinhThuc >= input.MinNgayChinhThucFilter)
						.WhereIf(input.MaxNgayChinhThucFilter != null, e => e.NgayChinhThuc <= input.MaxNgayChinhThucFilter)
						.WhereIf(input.MinSoNgayPhepFilter != null, e => e.SoNgayPhep >= input.MinSoNgayPhepFilter)
						.WhereIf(input.MaxSoNgayPhepFilter != null, e => e.SoNgayPhep <= input.MaxSoNgayPhepFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.BacLuongCodeFilter), e => e.BacLuongCode.ToLower() == input.BacLuongCodeFilter.ToLower().Trim())
						.WhereIf(input.MinLuongCoBanFilter != null, e => e.LuongCoBan >= input.MinLuongCoBanFilter)
						.WhereIf(input.MaxLuongCoBanFilter != null, e => e.LuongCoBan <= input.MaxLuongCoBanFilter)
						.WhereIf(input.MinLuongDongBHFilter != null, e => e.LuongDongBH >= input.MinLuongDongBHFilter)
						.WhereIf(input.MaxLuongDongBHFilter != null, e => e.LuongDongBH <= input.MaxLuongDongBHFilter)
						.WhereIf(input.MinSoCongChuanFilter != null, e => e.SoCongChuan >= input.MinSoCongChuanFilter)
						.WhereIf(input.MaxSoCongChuanFilter != null, e => e.SoCongChuan <= input.MaxSoCongChuanFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DonViSoCongChuanCodeFilter), e => e.DonViSoCongChuanCode.ToLower() == input.DonViSoCongChuanCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TkNganHangFilter), e => e.TkNganHang.ToLower() == input.TkNganHangFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.NganHangCodeFilter), e => e.NganHangCode.ToLower() == input.NganHangCodeFilter.ToLower().Trim())
					   .WhereIf(input.ThamGiaCongDoanFilter > -1, e => (input.ThamGiaCongDoanFilter == 1 && e.ThamGiaCongDoan) || (input.ThamGiaCongDoanFilter == 0 && !e.ThamGiaCongDoan))
						//.WhereIf(input.ThamGiaCongDoanFilter > -1,  e => Convert.ToInt32(e.ThamGiaCongDoan) == input.ThamGiaCongDoanFilter )
						.WhereIf(input.MinNgayThamGiaBHFilter != null, e => e.NgayThamGiaBH >= input.MinNgayThamGiaBHFilter)
						.WhereIf(input.MaxNgayThamGiaBHFilter != null, e => e.NgayThamGiaBH <= input.MaxNgayThamGiaBHFilter)
						.WhereIf(input.MinTyLeDongBHFilter != null, e => e.TyLeDongBH >= input.MinTyLeDongBHFilter)
						.WhereIf(input.MaxTyLeDongBHFilter != null, e => e.TyLeDongBH <= input.MaxTyLeDongBHFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.SoSoBHXHFilter), e => e.SoSoBHXH.ToLower() == input.SoSoBHXHFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.MaSoBHXHFilter), e => e.MaSoBHXH.ToLower() == input.MaSoBHXHFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.MaTinhCapFilter), e => e.MaTinhCap.ToLower() == input.MaTinhCapFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SoTheBHYTFilter), e => e.SoTheBHYT.ToLower() == input.SoTheBHYTFilter.ToLower().Trim())
						.WhereIf(input.MinNgayHetHanBHYTFilter != null, e => e.NgayHetHanBHYT >= input.MinNgayHetHanBHYTFilter)
						.WhereIf(input.MaxNgayHetHanBHYTFilter != null, e => e.NgayHetHanBHYT <= input.MaxNgayHetHanBHYTFilter)
						.WhereIf(input.MinNoiDangKyKCBIDFilter != null, e => e.NoiDangKyKCBID >= input.MinNoiDangKyKCBIDFilter)
						.WhereIf(input.MaxNoiDangKyKCBIDFilter != null, e => e.NoiDangKyKCBID <= input.MaxNoiDangKyKCBIDFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.MaSoNoiKCBFilter), e => e.MaSoNoiKCB.ToLower() == input.MaSoNoiKCBFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AUTH_STATUSFilter), e => e.AUTH_STATUS.ToLower() == input.AUTH_STATUSFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.RECORD_STATUSFilter), e => e.RECORD_STATUS.ToLower() == input.RECORD_STATUSFilter.ToLower().Trim())
						.WhereIf(input.MinMARKER_IDFilter != null, e => e.MARKER_ID >= input.MinMARKER_IDFilter)
						.WhereIf(input.MaxMARKER_IDFilter != null, e => e.MARKER_ID <= input.MaxMARKER_IDFilter)
						.WhereIf(input.MinCHECKER_IDFilter != null, e => e.CHECKER_ID >= input.MinCHECKER_IDFilter)
						.WhereIf(input.MaxCHECKER_IDFilter != null, e => e.CHECKER_ID <= input.MaxCHECKER_IDFilter)
						.WhereIf(input.MinAPPROVE_DTFilter != null, e => e.APPROVE_DT >= input.MinAPPROVE_DTFilter)
						.WhereIf(input.MaxAPPROVE_DTFilter != null, e => e.APPROVE_DT <= input.MaxAPPROVE_DTFilter);
			//var pagedAndFilteredHoSos = filteredHoSos
			//	.OrderBy(input.Sorting ?? "id asc")
			//	.PageBy(input);

			var tgd = _truongGiaoDichRepository.GetAll();
			var units = _organizationUnitRepository.GetAll();

			var hoSos = from o in filteredHoSos

						join gt in tgd.Where(x => x.Code == "GT") on o.GioiTinhCode equals gt.CDName into gtJoin
						from joinedgt in gtJoin.DefaultIfEmpty()

						join tthn in tgd.Where(x => x.Code == "TTHN") on o.TinhTrangHonNhanCode equals tthn.CDName into tthnJoin
						from joinedtthn in tthnJoin.DefaultIfEmpty()

						join tddt in tgd.Where(x => x.Code == "TDDT") on o.TrinhDoDaoTaoCode equals tddt.CDName into tddtJoin
						from joinedtddt in tddtJoin.DefaultIfEmpty()

						join lhd in tgd.Where(x => x.Code == "LHDG") on o.LoaiHopDongID equals lhd.Id into lhdJoin
						from joinelhd in lhdJoin.DefaultIfEmpty()

						join xl in tgd.Where(x => x.Code == "XLHL") on o.XepLoaiCode equals xl.CDName into xlJoin
						from joinedxl in xlJoin.DefaultIfEmpty()

						join ndt in _noiDaoTaoRepository.GetAll() on o.NoiDaoTaoID equals ndt.Id into ndtJoin
						from joinedndt in ndtJoin.DefaultIfEmpty()

						join tinhThanh1 in _tinhThanhRepository.GetAll() on o.TinhThanhIDHKTT equals tinhThanh1.Id into tinhThanh1Join
						from joinedtinhThanh1 in tinhThanh1Join.DefaultIfEmpty()

						join tinhThanh2 in _tinhThanhRepository.GetAll() on o.TinhThanhID equals tinhThanh2.Id into tinhThanh2Join
						from joinedtinhThanh2 in tinhThanh2Join.DefaultIfEmpty()

						join tinhThanh3 in _tinhThanhRepository.GetAll() on o.TinhThanhIDHN equals tinhThanh3.Id into tinhThanh3Join
						from joinedtinhThanh3 in tinhThanh3Join.DefaultIfEmpty()


						join hopdong in _hopDongRepository.GetAll() on o.LoaiHopDongID equals hopdong.Id into hopdongjoin
						from joinedthopdong in hopdongjoin.DefaultIfEmpty()

						join unit in units on o.DonViCongTacID.Value equals unit.Id into unitjoin
						from joinedtunit in unitjoin.DefaultIfEmpty()

						join noiDangKy in _dangKyKCBRepository.GetAll() on o.NoiDangKyKCBID.Value equals noiDangKy.Id into noiDangKyjoin
						from joinedtnoiDangKy in noiDangKyjoin.DefaultIfEmpty()

						select new GetHoSoForViewDto()
						{
							HoSo = new HoSoDto
							{
								MaNhanVien = o.MaNhanVien,
								HoVaTen = o.HoVaTen,
								AnhDaiDien = o.AnhDaiDien,
								GioiTinhValue = o.GioiTinhCode,
								NgaySinh = o.NgaySinh,
								MSTCaNhan = o.MSTCaNhan,
								DonViCongTacID = o.DonViCongTacID,
								ViTriCongViecValue = o.ViTriCongViecCode,
								DanToc = o.DanToc,
								TonGiao = o.TonGiao,
								QuocTich = o.QuocTich,
								SoCMND = o.SoCMND,
								NgayCap = o.NgayCap,
								NoiCap = o.NoiCap,
								NgayHetHan = o.NgayHetHan,
								TrinhDoVanHoa = o.TrinhDoVanHoa,
								TrinhDoDaoTaoCode = o.TrinhDoDaoTaoCode,
								NoiDaoTaoID = o.NoiDaoTaoID,
								Khoa = o.Khoa,
								ChuyenNganh = o.ChuyenNganh,
								NamTotNghiep = o.NamTotNghiep,
								XepLoaiCode = o.XepLoaiCode,
								TinhTrangHonNhanCode = o.TinhTrangHonNhanCode,
								TepDinhKem = o.TepDinhKem,
								DtDiDong = o.DtDiDong,
								DtCoQuan = o.DtCoQuan,
								DtNhaRieng = o.DtNhaRieng,
								DtKhac = o.DtKhac,
								EmailCaNhan = o.EmailCaNhan,
								EmailCoQuan = o.EmailCoQuan,
								EmailKhac = o.EmailKhac,
								NguyenQuan = o.NguyenQuan,
								TinhThanhID = o.TinhThanhID,
								NoiSinh = o.NoiSinh,
								Skype = o.Skype,
								Facebook = o.Facebook,
								QuocGiaHKTT = o.QuocGiaHKTT,
								TinhThanhIDHKTT = o.TinhThanhIDHKTT,
								DiaChiHKTT = o.DiaChiHKTT,
								SoSoHoKhau = o.SoSoHoKhau,
								MaSoHoGiaDinh = o.MaSoHoGiaDinh,
								LaChuHo = o.LaChuHo,
								QuocGiaHN = o.QuocGiaHN,
								TinhThanhIDHN = o.TinhThanhIDHN,
								DiaChiHN = o.DiaChiHN,
								HoVaTenLHKC = o.HoVaTenLHKC,
								QuanHeLHKC = o.QuanHeLHKC,
								DtDiDongLHKC = o.DtDiDongLHKC,
								DtNhaRiengLHKC = o.DtNhaRiengLHKC,
								EmailLHKC = o.EmailLHKC,
								DiaChiLHKC = o.DiaChiLHKC,
								MaChamCong = o.MaChamCong,
								ChucDanh = o.ChucDanh,
								Cap = o.Cap,
								Bac = o.Bac,
								TrangThaiLamViecCode = o.TrangThaiLamViecCode,
								QuanLyTrucTiep = o.QuanLyTrucTiep,
								QuanLyGianTiep = o.QuanLyGianTiep,
								DiaDiemLamViecCode = o.DiaDiemLamViecCode,
								SoSoQLLaoDong = o.SoSoQLLaoDong,
								HopDongValue = o.LoaiHopDongID,
								NgayTapSu = o.NgayTapSu,
								NgayThuViec = o.NgayThuViec,
								NgayChinhThuc = o.NgayChinhThuc,
								SoNgayPhep = o.SoNgayPhep,
								BacLuongCode = o.BacLuongCode,
								LuongCoBan = o.LuongCoBan,
								LuongDongBH = o.LuongDongBH,
								SoCongChuan = o.SoCongChuan,
								DonViSoCongChuanCode = o.DonViSoCongChuanCode,
								TkNganHang = o.TkNganHang,
								NganHangCode = o.NganHangCode,
								ThamGiaCongDoan = o.ThamGiaCongDoan,
								NgayThamGiaBH = o.NgayThamGiaBH,
								TyLeDongBH = o.TyLeDongBH,
								SoSoBHXH = o.SoSoBHXH,
								MaSoBHXH = o.MaSoBHXH,
								MaTinhCap = o.MaTinhCap,
								SoTheBHYT = o.SoTheBHYT,
								NgayHetHanBHYT = o.NgayHetHanBHYT,
								NoiDangKyKCBID = o.NoiDangKyKCBID,
								MaSoNoiKCB = o.MaSoNoiKCB,
								AUTH_STATUS = o.AUTH_STATUS,
								RECORD_STATUS = o.RECORD_STATUS,
								MARKER_ID = o.MARKER_ID,
								CHECKER_ID = o.CHECKER_ID,
								APPROVE_DT = o.APPROVE_DT,
								Id = o.Id
							},

							GioiTinhValue = joinedgt == null ? "" : joinedgt.Value.ToString(),
							TinhTrangHonNhanValue = joinedtthn == null ? "" : joinedtthn.Value.ToString(),
							LoaiHopDongValue = joinelhd == null ? "" : joinelhd.Value.ToString(),
							TrinhDoDaoTaoValue = joinedtddt == null ? "" : joinedtddt.Value.ToString(),
							XepLoaiValue = joinedxl == null ? "" : joinedxl.Value.ToString(),
							NoiDaoTaoValue = joinedndt == null ? "" : joinedndt.TenNoiDaoTao.ToString(),
							TinhThanhIDHKTTValue = joinedtinhThanh1 == null ? "" : joinedtinhThanh1.TenTinhThanh.ToString(),
							TinhThanhValue = joinedtinhThanh2 == null ? "" : joinedtinhThanh2.TenTinhThanh.ToString(),
							TinhThanhIDHNValue = joinedtinhThanh3 == null ? "" : joinedtinhThanh3.TenTinhThanh.ToString(),
							HopDongValue = joinedthopdong == null ? "" : joinedthopdong.TenHopDong.ToString(),
					        DonViCongTacValue = joinedtunit == null ? "" : joinedtunit.DisplayName.ToString(),
							NoiDangKyValue = joinedtnoiDangKy == null ? "" : joinedtnoiDangKy.TenNoiKCB.ToString(),
						};


			var hoSoListDtos = await hoSos.ToListAsync();

			return _hoSosExcelExporter.ExportToFile(hoSoListDtos);
		}
		public async Task<string> importToExcel(string currentTime, string path)
		{

			try
			{

				string date = DateTime.Today.ToString("dd-MM-yyyy");
				SpreadsheetInfo.SetLicense("ELAP-G41W-CZA2-XNNC");
				var path1 = Path.Combine(_env.WebRootPath + "\\" + date + "\\" + currentTime + "\\" + path);
				var workbook = ExcelFile.Load(path1);

				var dataTable = new DataTable();



				dataTable.Columns.Add("Mã chấm công", typeof(string));
				dataTable.Columns.Add("Mã nhân viên", typeof(string));
				dataTable.Columns.Add("Họ và tên", typeof(string));
				dataTable.Columns.Add("Ngày sinh", typeof(string));
				dataTable.Columns.Add("Trình độ", typeof(string));
				dataTable.Columns.Add("Giới tính", typeof(string));
				dataTable.Columns.Add("Số CMND", typeof(string));
				dataTable.Columns.Add("Ngày cấp", typeof(string));
				dataTable.Columns.Add("Nơi cấp", typeof(string));
				dataTable.Columns.Add("MST cá nhân", typeof(string));
				dataTable.Columns.Add("SĐT", typeof(string));	
				dataTable.Columns.Add("Địa chỉ thường trú", typeof(string));
				//dataTable.Columns.Add("Vị trí công việc", typeof(string));
				dataTable.Columns.Add("Chức vụ", typeof(string));
				dataTable.Columns.Add("Phòng ban", typeof(string));
				dataTable.Columns.Add("Hợp đồng hiện tại", typeof(string));
				dataTable.Columns.Add("Ngày hết hạn", typeof(string));
				dataTable.Columns.Add("Ngày vào cty", typeof(string));
				dataTable.Columns.Add("Ngân hàng", typeof(string));
				dataTable.Columns.Add("Chỗ ngồi", typeof(string));
				dataTable.Columns.Add("STK", typeof(string));
				dataTable.Columns.Add("Số HĐ", typeof(string));
				dataTable.Columns.Add("Tên CTY", typeof(string));


				// Select the first worksheet from the file.
				var worksheet = workbook.Worksheets[0];
				var options = new ExtractToDataTableOptions(1, 0, 10000);
				options.ExtractDataOptions = ExtractDataOptions.StopAtFirstEmptyRow;
				options.ExcelCellToDataTableCellConverting += (sender, e) =>
				{
					if (!e.IsDataTableValueValid)
					{

						e.DataTableValue = e.ExcelCell.Value == null ? null : e.ExcelCell.Value.ToString();
						e.Action = ExtractDataEventAction.Continue;
					}
				};
				worksheet.ExtractToDataTable(dataTable, options);

				foreach (DataRow row in dataTable.Rows)
				{

					string mcc, mnv, hoten, gioitinh, soCMND, mst, sdt, diachiTK, chuvu, pb, hdHt, chongoi, stk, soHD, ns, nc, nhh , nvl ;
					DateTime Ngay_Sinh, Ngay_Cap, Ngay_Het_Han, Ngay_Vao_Lam;
					var hoSo = new CreateOrEditHoSoDto();
					var TGD = _truongGiaoDichRepository.GetAll();
					var dvct = _organizationUnitRepository.GetAll();

					hoSo.MaChamCong = row[0].ToString();
					hoSo.HoVaTen = row[2].ToString();

					//if (row[2].ToString().IsNullOrEmpty())
					//{
					//	throw new UserFriendlyException(L(name: "Họ tên không được để trống"));
					//}
					//else
					//{

					//	hoSo.HoVaTen = row[2].ToString();
					//}

					ns = row[3].ToString();
					if (ns != "")
					{
						try
						{
							Ngay_Sinh = Convert.ToDateTime(ns);
							hoSo.NgaySinh = Ngay_Sinh;

						}
						catch (Exception ex)
						{
							hoSo.NgaySinh = DateTime.MinValue;
						}

					}
					else
					{
						hoSo.NgaySinh = null;
					}

					hoSo.TrinhDoDaoTaoCode = row[4].ToString();
					hoSo.GioiTinhCode = row[5].ToString();
					//gioitinh = row[5].ToString();
					//if (gioitinh != null)

					//{
					//	//  hoSo.GioiTinhCode = TGD.FirstOrDefault(x => x.Code == "GT" && x.Value == GioiTinh).CDName;
					//	var customerQuery = from tgd in TGD
					//						where tgd.Code == "GT" && tgd.Value == gioitinh
					//						select tgd.CDName;

					//	foreach (var Customer in customerQuery)
					//	{
					//		hoSo.GioiTinhCode = Customer;
					//	}
					//}
					//else
					//{
					//	hoSo.GioiTinhCode = null;
					//}

					hoSo.SoCMND = row[6].ToString();
					//soCMND = row[5].ToString();
					//if (CheckCMND(soCMND) && !soCMND.IsNullOrWhiteSpace())
					//{
					//	throw new UserFriendlyException(L("Số CMND đã bị trùng"));
					//}
					//else
					//{

					//	hoSo.SoCMND = soCMND;
					//}

					nc = row[7].ToString();
					if (nc != "")
					{
						try
						{
							Ngay_Cap = Convert.ToDateTime(nc);
							hoSo.NgayCap = Ngay_Cap;

						}
						catch (Exception ex)
						{
							hoSo.NgayCap = DateTime.MinValue;
						}

					}
					else
					{
						hoSo.NgayCap = null;
					}

					hoSo.NoiCap = row[8].ToString();
					hoSo.MSTCaNhan = row[9].ToString();
					hoSo.DtDiDong = row[10].ToString();
					hoSo.DiaChiHKTT = row[11].ToString();
					hoSo.ChucDanh = row[12].ToString();
					hoSo.DonViCongTacName = row[13].ToString();
					hoSo.HopDongHienTai = row[14].ToString();

					nhh = row[15].ToString();
					if (nhh != "")
					{
						try
						{
							Ngay_Het_Han = Convert.ToDateTime(nhh);
							hoSo.NgayHetHan = Ngay_Het_Han;

						}
						catch (Exception ex)
						{
							hoSo.NgayHetHan = DateTime.MinValue;
						}

					}

					else
					{
						hoSo.NgayHetHan = null;
					}
					nvl = row[16].ToString();
					if (nvl != "")
					{
						try
						{
							Ngay_Vao_Lam = Convert.ToDateTime(nvl);
							hoSo.NgayThuViec = Ngay_Vao_Lam;

						}
						catch (Exception ex)
						{
							hoSo.NgayThuViec = DateTime.MinValue;
						}

					}
				
					hoSo.NganHangCode = row[17].ToString();
					hoSo.ChoNgoi = row[18].ToString();
					hoSo.TkNganHang = row[19].ToString();
			    	hoSo.SoHD = row[20].ToString();
					hoSo.TenCty = row[21].ToString();

					//var hoSos = ObjectMapper.Map<hoSo>(hoSo);
					await CreateOrEdit(hoSo);

					// await _hoSoRepository.InsertAsync(hoSos);


				}

				return mes;
			}

			catch (Exception ex)
			{

				Logger.Error("+hoten" );
				Logger.Error(ex.StackTrace);
				return "";
			}
		}

	}
}