

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
using System.Data.SqlClient;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Hinnova.Configuration;
using Microsoft.Extensions.Configuration;
using Abp.Organizations;

namespace Hinnova.QLNS
{
    //[AbpAuthorize(AppPermissions.Pages_HopDongs)]
    public class HopDongsAppService : HinnovaAppServiceBase, IHopDongsAppService
    {
        private readonly IRepository<HopDong> _hopDongRepository;
        private readonly IRepository<UngVien> _ungVienRepository;
        private readonly IHopDongsExcelExporter _hopDongsExcelExporter;
        private readonly IRepository<TruongGiaoDich> _truongGiaoDichRepository;
        private readonly string connectionString;
        private readonly IWebHostEnvironment _env;
        IRepository<OrganizationUnit, long> _organizationUnitRepository;

        public HopDongsAppService(IWebHostEnvironment env, IRepository<OrganizationUnit, long> organizationUnitRepository, IRepository<UngVien> ungVienRepository, IWebHostEnvironment hostingEnvironment, IRepository<TruongGiaoDich> truongGiaoDichRepository, IRepository<HopDong> hopDongRepository, IHopDongsExcelExporter hopDongsExcelExporter)
        {
            _hopDongRepository = hopDongRepository;
            _env = hostingEnvironment;
            _organizationUnitRepository = organizationUnitRepository;
            connectionString = env.GetAppConfiguration().GetConnectionString("Default");
            _ungVienRepository = ungVienRepository;
            _hopDongsExcelExporter = hopDongsExcelExporter;
               
        _truongGiaoDichRepository = truongGiaoDichRepository;
        }

        public async Task<PagedResultDto<GetHopDongForViewDto>> GetAll(GetAllHopDongsInput input)
        {

            var filteredHopDongs = _hopDongRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.HoTenNhanVien.Contains(input.Filter) || e.ViTriCongViecCode.Contains(input.Filter) || e.SoHopDong.Contains(input.Filter) || e.TenHopDong.Contains(input.Filter) || e.LoaiHopDongCode.Contains(input.Filter) || e.HinhThucLamViecCode.Contains(input.Filter) || e.NguoiDaiDienCongTy.Contains(input.Filter) || e.ChucDanh.Contains(input.Filter) || e.TrichYeu.Contains(input.Filter) || e.TepDinhKem.Contains(input.Filter) || e.GhiChu.Contains(input.Filter) || e.RECORD_STATUS.Contains(input.Filter) || e.AUTH_STATUS.Contains(input.Filter) || e.ThoiHanHopDong.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.HoTenNhanVienFilter), e => e.HoTenNhanVien.ToLower() == input.HoTenNhanVienFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ViTriCongViecCodeFilter), e => e.ViTriCongViecCode.ToLower() == input.ViTriCongViecCodeFilter.ToLower().Trim())
                        .WhereIf(input.MinNgayKyFilter != null, e => e.NgayKy >= input.MinNgayKyFilter)
                        .WhereIf(input.MaxNgayKyFilter != null, e => e.NgayKy <= input.MaxNgayKyFilter)
                        .WhereIf(input.MinDonViCongTacIDFilter != null, e => e.DonViCongTacID >= input.MinDonViCongTacIDFilter)
                        .WhereIf(input.MaxDonViCongTacIDFilter != null, e => e.DonViCongTacID <= input.MaxDonViCongTacIDFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TenHopDongFilter), e => e.TenHopDong.ToLower() == input.TenHopDongFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LoaiHopDongCodeFilter), e => e.LoaiHopDongCode.ToLower() == input.LoaiHopDongCodeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.HinhThucLamViecCodeFilter), e => e.HinhThucLamViecCode.ToLower() == input.HinhThucLamViecCodeFilter.ToLower().Trim())
                        .WhereIf(input.MinNgayCoHieuLucFilter != null, e => e.NgayCoHieuLuc >= input.MinNgayCoHieuLucFilter)
                        .WhereIf(input.MaxNgayCoHieuLucFilter != null, e => e.NgayCoHieuLuc <= input.MaxNgayCoHieuLucFilter)
                        .WhereIf(input.MinNgayHetHanFilter != null, e => e.NgayHetHan >= input.MinNgayHetHanFilter)
                        .WhereIf(input.MaxNgayHetHanFilter != null, e => e.NgayHetHan <= input.MaxNgayHetHanFilter)
                        .WhereIf(input.MinLuongCoBanFilter != null, e => e.LuongCoBan >= input.MinLuongCoBanFilter)
                        .WhereIf(input.MaxLuongCoBanFilter != null, e => e.LuongCoBan <= input.MaxLuongCoBanFilter)
                        .WhereIf(input.MinLuongDongBaoHiemFilter != null, e => e.LuongDongBaoHiem >= input.MinLuongDongBaoHiemFilter)
                        .WhereIf(input.MaxLuongDongBaoHiemFilter != null, e => e.LuongDongBaoHiem <= input.MaxLuongDongBaoHiemFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ChucDanhFilter), e => e.ChucDanh.ToLower() == input.ChucDanhFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrichYeuFilter), e => e.TrichYeu.ToLower() == input.TrichYeuFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RECORD_STATUSFilter), e => e.RECORD_STATUS.ToLower() == input.RECORD_STATUSFilter.ToLower().Trim())
                        .WhereIf(input.MinMARKER_IDFilter != null, e => e.MARKER_ID >= input.MinMARKER_IDFilter)
                        .WhereIf(input.MaxMARKER_IDFilter != null, e => e.MARKER_ID <= input.MaxMARKER_IDFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AUTH_STATUSFilter), e => e.AUTH_STATUS.ToLower() == input.AUTH_STATUSFilter.ToLower().Trim())
                        .WhereIf(input.MinCHECKER_IDFilter != null, e => e.CHECKER_ID >= input.MinCHECKER_IDFilter)
                        .WhereIf(input.MaxCHECKER_IDFilter != null, e => e.CHECKER_ID <= input.MaxCHECKER_IDFilter)
                        .WhereIf(input.MinAPPROVE_DTFilter != null, e => e.APPROVE_DT >= input.MinAPPROVE_DTFilter)
                        .WhereIf(input.MaxAPPROVE_DTFilter != null, e => e.APPROVE_DT <= input.MaxAPPROVE_DTFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ThoiHanHopDongFilter), e => e.ThoiHanHopDong.ToLower() == input.ThoiHanHopDongFilter.ToLower().Trim());

            var pagedAndFilteredHopDongs = filteredHopDongs
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);
            var units = _organizationUnitRepository.GetAll();
            var tgd = _truongGiaoDichRepository.GetAll();

            var hopDongs = from o in pagedAndFilteredHopDongs
                           join vtcv in tgd.Where(x => x.Code == "VTCV")
                           on o.ViTriCongViecCode equals vtcv.CDName into vtcvJoin
                           from joinedvtcv in vtcvJoin.DefaultIfEmpty()

                           //join lhdgs in tgd.Where(x => x.Code == "LHDGS") on o.LoaiHopDongCode equals lhdgs.CDName into lhdgsJoin
                           //from joinedlhdgs in lhdgsJoin.DefaultIfEmpty()

                           //join lhdgb in tgd.Where(x => x.Code == "LHDGB") on o.LoaiHopDongCode equals lhdgb.CDName into lhdgbJoin
                           //from joinedlhdhb in lhdgbJoin.DefaultIfEmpty()

                           join lhd in tgd.Where(x => x.Code == "LHDG") on o.LoaiHopDongCode equals lhd.CDName into lhdJoin
                           from joinedlhd in lhdJoin.DefaultIfEmpty()

                           join htlv in tgd.Where(x => x.Code == "HTLV") on o.HinhThucLamViecCode equals htlv.CDName into htlvJoin
                           from joinedhtlv in htlvJoin.DefaultIfEmpty()

                           join thhd in tgd.Where(x => x.Code == "THHD") on o.ThoiHanHopDong equals thhd.CDName into thhdJoin
                           from joinedthhd in thhdJoin.DefaultIfEmpty()

                           join unit in units on o.DonViCongTacID.Value equals unit.Id into unitjoin
                           from joinedtunit in unitjoin.DefaultIfEmpty()
                               //join ungvien in _ungVienRepository.GetAll() on o.NhanVienId equals ungvien.Id into ungvienjoin
                               //from joinedtungvien in ungvienjoin.DefaultIfEmpty()

                           select new GetHopDongForViewDto()
                           {
                               HopDong = new HopDongDto
                               {
                                   NhanVienId = o.NhanVienId,
                                   HoTenNhanVien = o.HoTenNhanVien,
                                   ViTriCongViecCode = o.ViTriCongViecCode,
                                   SoHopDong = o.SoHopDong,
                                   NgayKy = o.NgayKy,
                                   DonViCongTacID = o.DonViCongTacID,
                                   TenHopDong = o.TenHopDong,
                                   LoaiHopDongCode = o.LoaiHopDongCode,
                                   HinhThucLamViecCode = o.HinhThucLamViecCode,
                                   NgayCoHieuLuc = o.NgayCoHieuLuc,
                                   NgayHetHan = o.NgayHetHan,
                                   LuongCoBan = (o.LuongCoBan ?? 0).ToString("#,###"),
                                   LuongDongBaoHiem = (o.LuongDongBaoHiem ?? 0).ToString("#,###"),
                                   TyLeHuongLuong = o.TyLeHuongLuong,
                                   NguoiDaiDienCongTy = o.NguoiDaiDienCongTy,
                                   ChucDanh = o.ChucDanh,
                                   TrichYeu = o.TrichYeu,
                                   TepDinhKem = o.TepDinhKem,
                                   GhiChu = o.GhiChu,
                                   RECORD_STATUS = o.RECORD_STATUS,
                                   MARKER_ID = o.MARKER_ID,
                                   AUTH_STATUS = o.AUTH_STATUS,
                                   CHECKER_ID = o.CHECKER_ID,
                                   APPROVE_DT = o.APPROVE_DT,
                                   ThoiHanHopDong = o.ThoiHanHopDong,
                                   Id = o.Id
                               },
                               DonViCongTacValue = joinedtunit == null ? "" : joinedtunit.DisplayName.ToString(),
                               ThoiHanhopDongTaoValue = joinedthhd == null ? "" : joinedthhd.Value.ToString(),
                                LoaiHopDongValue = joinedlhd == null ? "" : joinedlhd.Value.ToString(),
                               HinhThucLamViecValue= joinedhtlv == null ? "" : joinedhtlv.Value.ToString(),
                           };

            var totalCount = await filteredHopDongs.CountAsync();

            return new PagedResultDto<GetHopDongForViewDto>(
                totalCount,
                await hopDongs.ToListAsync()
            );
        }
        public List<HopDongDto> GetAllHopDong()
        {
            return ObjectMapper.Map<List<HopDongDto>>(_hopDongRepository.GetAll().ToList());
        }


        public async Task<List<TruongGiaoDichDto>> GetInFoLHD(string name)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    await conn.OpenAsync();
                }
                var result = await conn.QueryAsync<TruongGiaoDichDto>(sql: "dbo.GetInFoLHD", param: new { name }, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }
        public async Task<GetHopDongForViewDto> GetHopDongForView(int id)
        {
            var hopDong = await _hopDongRepository.GetAsync(id);

            var output = new GetHopDongForViewDto { HopDong = ObjectMapper.Map<HopDongDto>(hopDong) };

            return output;
        }

        //[AbpAuthorize(AppPermissions.Pages_HopDongs_Edit)]
        public async Task<GetHopDongForEditOutput> GetHopDongForEdit(EntityDto input)
        {
            var hopDong = await _hopDongRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetHopDongForEditOutput { HopDong = ObjectMapper.Map<CreateOrEditHopDongDto>(hopDong) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditHopDongDto input)
        {
            if (input.Id == null)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                        await conn.OpenAsync();
                    var tableName = "HopDong";
                    var result = await conn.QueryAsync<string>(sql: "exec SYS_CodeMasters_Gen_LHD " + tableName);
                    input.SoHopDong = result.ToList().First();
                    await Create(input);
                }
            }
            else
            {
                await Update(input);
            }
        }

        //[AbpAuthorize(AppPermissions.Pages_HopDongs_Create)]
        protected virtual async Task Create(CreateOrEditHopDongDto input)
        {
            var hopDong = ObjectMapper.Map<HopDong>(input);



            await _hopDongRepository.InsertAsync(hopDong);
        }

        //[AbpAuthorize(AppPermissions.Pages_HopDongs_Edit)]
        protected virtual async Task Update(CreateOrEditHopDongDto input)
        {
            var hopDong = await _hopDongRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, hopDong);
        }

        //[AbpAuthorize(AppPermissions.Pages_HopDongs_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _hopDongRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetHopDongsToExcel(GetAllHopDongsForExcelInput input)
        {

            var filteredHopDongs = _hopDongRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.HoTenNhanVien.Contains(input.Filter) || e.ViTriCongViecCode.Contains(input.Filter) || e.SoHopDong.Contains(input.Filter) || e.TenHopDong.Contains(input.Filter) || e.LoaiHopDongCode.Contains(input.Filter) || e.HinhThucLamViecCode.Contains(input.Filter) || e.NguoiDaiDienCongTy.Contains(input.Filter) || e.ChucDanh.Contains(input.Filter) || e.TrichYeu.Contains(input.Filter) || e.TepDinhKem.Contains(input.Filter) || e.GhiChu.Contains(input.Filter) || e.RECORD_STATUS.Contains(input.Filter) || e.AUTH_STATUS.Contains(input.Filter) || e.ThoiHanHopDong.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.HoTenNhanVienFilter), e => e.HoTenNhanVien.ToLower() == input.HoTenNhanVienFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ViTriCongViecCodeFilter), e => e.ViTriCongViecCode.ToLower() == input.ViTriCongViecCodeFilter.ToLower().Trim())
                        .WhereIf(input.MinNgayKyFilter != null, e => e.NgayKy >= input.MinNgayKyFilter)
                        .WhereIf(input.MaxNgayKyFilter != null, e => e.NgayKy <= input.MaxNgayKyFilter)
                        .WhereIf(input.MinDonViCongTacIDFilter != null, e => e.DonViCongTacID >= input.MinDonViCongTacIDFilter)
                        .WhereIf(input.MaxDonViCongTacIDFilter != null, e => e.DonViCongTacID <= input.MaxDonViCongTacIDFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TenHopDongFilter), e => e.TenHopDong.ToLower() == input.TenHopDongFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LoaiHopDongCodeFilter), e => e.LoaiHopDongCode.ToLower() == input.LoaiHopDongCodeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.HinhThucLamViecCodeFilter), e => e.HinhThucLamViecCode.ToLower() == input.HinhThucLamViecCodeFilter.ToLower().Trim())
                        .WhereIf(input.MinNgayCoHieuLucFilter != null, e => e.NgayCoHieuLuc >= input.MinNgayCoHieuLucFilter)
                        .WhereIf(input.MaxNgayCoHieuLucFilter != null, e => e.NgayCoHieuLuc <= input.MaxNgayCoHieuLucFilter)
                        .WhereIf(input.MinNgayHetHanFilter != null, e => e.NgayHetHan >= input.MinNgayHetHanFilter)
                        .WhereIf(input.MaxNgayHetHanFilter != null, e => e.NgayHetHan <= input.MaxNgayHetHanFilter)
                        .WhereIf(input.MinLuongCoBanFilter != null, e => e.LuongCoBan >= input.MinLuongCoBanFilter)
                        .WhereIf(input.MaxLuongCoBanFilter != null, e => e.LuongCoBan <= input.MaxLuongCoBanFilter)
                        .WhereIf(input.MinLuongDongBaoHiemFilter != null, e => e.LuongDongBaoHiem >= input.MinLuongDongBaoHiemFilter)
                        .WhereIf(input.MaxLuongDongBaoHiemFilter != null, e => e.LuongDongBaoHiem <= input.MaxLuongDongBaoHiemFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ChucDanhFilter), e => e.ChucDanh.ToLower() == input.ChucDanhFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrichYeuFilter), e => e.TrichYeu.ToLower() == input.TrichYeuFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RECORD_STATUSFilter), e => e.RECORD_STATUS.ToLower() == input.RECORD_STATUSFilter.ToLower().Trim())
                        .WhereIf(input.MinMARKER_IDFilter != null, e => e.MARKER_ID >= input.MinMARKER_IDFilter)
                        .WhereIf(input.MaxMARKER_IDFilter != null, e => e.MARKER_ID <= input.MaxMARKER_IDFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AUTH_STATUSFilter), e => e.AUTH_STATUS.ToLower() == input.AUTH_STATUSFilter.ToLower().Trim())
                        .WhereIf(input.MinCHECKER_IDFilter != null, e => e.CHECKER_ID >= input.MinCHECKER_IDFilter)
                        .WhereIf(input.MaxCHECKER_IDFilter != null, e => e.CHECKER_ID <= input.MaxCHECKER_IDFilter)
                        .WhereIf(input.MinAPPROVE_DTFilter != null, e => e.APPROVE_DT >= input.MinAPPROVE_DTFilter)
                        .WhereIf(input.MaxAPPROVE_DTFilter != null, e => e.APPROVE_DT <= input.MaxAPPROVE_DTFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ThoiHanHopDongFilter), e => e.ThoiHanHopDong.ToLower() == input.ThoiHanHopDongFilter.ToLower().Trim());
            var units = _organizationUnitRepository.GetAll();
            var tgd = _truongGiaoDichRepository.GetAll();

            var hopDongs = from o in filteredHopDongs
                           join vtcv in tgd.Where(x => x.Code == "VTCV")
                           on o.ViTriCongViecCode equals vtcv.CDName into vtcvJoin
                           from joinedvtcv in vtcvJoin.DefaultIfEmpty()

                           //join lhd in tgd.Where(x => x.Code == "LHD") on o.LoaiHopDongCode equals lhd.CDName into lhdJoin
                           //from joinedlhd in lhdJoin.DefaultIfEmpty()

                           join lhd in tgd.Where(x => x.Code == "LHDG") on o.LoaiHopDongCode equals lhd.CDName into lhdJoin
                           from joinedlhd in lhdJoin.DefaultIfEmpty()

                           join htlv in tgd.Where(x => x.Code == "HTLV") on o.HinhThucLamViecCode equals htlv.CDName into htlvJoin
                           from joinedhtlv in htlvJoin.DefaultIfEmpty()

                           join thhd in tgd.Where(x => x.Code == "THHD") on o.ThoiHanHopDong equals thhd.CDName into thhdJoin
                           from joinedthhd in thhdJoin.DefaultIfEmpty()

                           join unit in units on o.DonViCongTacID.Value equals unit.Id into unitjoin
                           from joinedtunit in unitjoin.DefaultIfEmpty()
                  
                         
                         select new GetHopDongForViewDto()
                         {
                             HopDong = new HopDongDto
                             {
                                 NhanVienId = o.NhanVienId,
                                 HoTenNhanVien = o.HoTenNhanVien,
                                 ViTriCongViecCode = o.ViTriCongViecCode,
                                 SoHopDong = o.SoHopDong,
                                 NgayKy = o.NgayKy,
                                 DonViCongTacID = o.DonViCongTacID,
                                 TenHopDong = o.TenHopDong,
                                 LoaiHopDongCode = o.LoaiHopDongCode,
                                 HinhThucLamViecCode = o.HinhThucLamViecCode,
                                 NgayCoHieuLuc = o.NgayCoHieuLuc,
                                 NgayHetHan = o.NgayHetHan,
                                 LuongCoBan = (o.LuongCoBan ?? 0).ToString("#,###"),
                                 LuongDongBaoHiem = (o.LuongDongBaoHiem ?? 0).ToString("#,###"),
                                 TyLeHuongLuong = o.TyLeHuongLuong,
                                 NguoiDaiDienCongTy = o.NguoiDaiDienCongTy,
                                 ChucDanh = o.ChucDanh,
                                 TrichYeu = o.TrichYeu,
                                 TepDinhKem = o.TepDinhKem,
                                 GhiChu = o.GhiChu,
                                 RECORD_STATUS = o.RECORD_STATUS,
                                 MARKER_ID = o.MARKER_ID,
                                 AUTH_STATUS = o.AUTH_STATUS,
                                 CHECKER_ID = o.CHECKER_ID,
                                 APPROVE_DT = o.APPROVE_DT,
                                 ThoiHanHopDong = o.ThoiHanHopDong,
                                 Id = o.Id
                             },
                             DonViCongTacValue = joinedtunit == null ? "" : joinedtunit.DisplayName.ToString(),
                             ThoiHanhopDongTaoValue = joinedthhd == null ? "" : joinedthhd.Value.ToString(),
                             LoaiHopDongValue = joinedlhd == null ? "" : joinedlhd.Value.ToString(),
                             HinhThucLamViecValue = joinedhtlv == null ? "" : joinedhtlv.Value.ToString(),
                         };


            var hopDongListDtos = await hopDongs.ToListAsync();

            return _hopDongsExcelExporter.ExportToFile(hopDongListDtos);
        }



    }
}