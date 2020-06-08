

using System;
using System.Data;
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
using Abp.Dapper.Repositories;
using Microsoft.AspNetCore.Hosting;
using Hinnova.Configuration;
using Microsoft.Extensions.Configuration;
using System.Data;
using Dapper;
using System.IO;
using Abp.UI;

using System.Text;
using GemBox.Spreadsheet;
using Hinnova.Utils.EmailSenders;
using Hinnova.Configuration.Host.Dto;
using System.Net.Mime;
using System.Net.Mail;

namespace Hinnova.QLNS
{
    //[AbpAuthorize(AppPermissions.Pages_UngViens)]
    public class UngViensAppService : HinnovaAppServiceBase, IUngViensAppService
    {

        private readonly IWebHostEnvironment _env;
        private CoreEmailSender CoreEmailSender;
        private readonly IRepository<UngVien> _ungVienRepository;
        private readonly IRepository<TruongGiaoDich> _truongGiaoDichRepository;
        private readonly IRepository<TinhThanh> _tinhThanhRepository;
        private readonly IRepository<NoiDaoTao> _noiDaoTaoRepository;
        private readonly IUngViensExcelExporter _ungViensExcelExporter;
        private readonly string connectionString;
        private readonly string mes;


        public UngViensAppService(IRepository<NoiDaoTao> noiDaoTaoRepository, IWebHostEnvironment hostingEnvironment, IRepository<TinhThanh> tinhThanhRepository, IRepository<TruongGiaoDich> truongGiaoDichRepository, IWebHostEnvironment env, IRepository<UngVien> ungVienRepository, IUngViensExcelExporter ungViensExcelExporter)
        {
            _env = hostingEnvironment;
            _ungVienRepository = ungVienRepository;
            _ungViensExcelExporter = ungViensExcelExporter;
            //CoreEmailSender = coreEmailSender;
            _truongGiaoDichRepository = truongGiaoDichRepository;
            _noiDaoTaoRepository = noiDaoTaoRepository;
            _tinhThanhRepository = tinhThanhRepository;
            connectionString = env.GetAppConfiguration().GetConnectionString("Default");
        }

        public bool CheckCMND(string cmnd)
        {
            //if (cmnd.IsNullOrEmpty())
            //{
            //    throw new UserFriendlyException(L("Số CMND  không được trống"));
            //}

            var x = _ungVienRepository.GetAll().Any(k => k.SoCMND == cmnd && k.IsDeleted == false);
            return x;
        }

        public async Task SendEmailKH(SendTestEmailInput input)

        {

            string date = DateTime.Today.ToString("dd-MM-yyyy");
            EmailInfo emailInfo = new EmailInfo();
            CoreEmailSender = new CoreEmailSender();
            MailAddress to = new MailAddress(input.EmailAddress);
            MailAddress from = new MailAddress(input.mailForm);

            MailMessage message = new MailMessage(from, to);
            message.Subject = input.subject;
            message.Body = input.body;
            message.Dispose();
            if (input.filedinhkem == null) 
            {
                emailInfo.dataAttach = null;
                emailInfo.isAttach = false;
            }
            else
            {
                var path = Path.Combine(_env.WebRootPath, date, input.curentTime, input.filedinhkem);
                MemoryStream ms = new MemoryStream(File.ReadAllBytes(path));
                emailInfo.dataAttach = ms;
                emailInfo.isAttach = true;
            }

            emailInfo.nameAttach = input.filedinhkem;
            emailInfo.Subj = input.subject;
            emailInfo.Message = input.body;
            emailInfo.ToEmail = input.EmailAddress;
            emailInfo.smtpAddress = input.diaChiIP;
            emailInfo.portNumber = input.congSMTP;
            emailInfo.enableSSL = false;
            emailInfo.password = input.matKhau;
            emailInfo.emailFrom = input.mailForm;
            emailInfo.displayName = input.tenTruyCap;

        //ms.Position = 0;
        CoreEmailSender.SendEmail(emailInfo);
        }


        public List<UngVienDto> GetAllCMND()
        {
            return _ungVienRepository.GetAll().Where(t => t.IsDeleted == false).Select(t => new UngVienDto { Id = t.Id, SoCMND = t.SoCMND }).ToList();
        }

        public async Task<PagedResultDto<GetUngVienForViewDto>> GetAll(GetAllUngViensInput input)
        {

            var filteredUngViens = _ungVienRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.MaUngVien.Contains(input.Filter) || e.HoVaTen.Contains(input.Filter) || e.ViTriUngTuyenCode.Contains(input.Filter) || e.KenhTuyenDungCode.Contains(input.Filter) || e.GioiTinhCode.Contains(input.Filter) || e.SoCMND.Contains(input.Filter) || e.NoiCap.Contains(input.Filter) || e.TinhTrangHonNhanCode.Contains(input.Filter) || e.TrinhDoDaoTaoCode.Contains(input.Filter) || e.TrinhDoVanHoa.Contains(input.Filter) || e.Khoa.Contains(input.Filter) || e.ChuyenNganh.Contains(input.Filter) || e.XepLoaiCode.Contains(input.Filter) || e.TrangThaiCode.Contains(input.Filter) || e.TienDoTuyenDungCode.Contains(input.Filter) || e.TepDinhKem.Contains(input.Filter) || e.RECORD_STATUS.Contains(input.Filter) || e.AUTH_STATUS.Contains(input.Filter) || e.DienThoai.Contains(input.Filter) || e.Email.Contains(input.Filter) || e.DiaChi.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MaUngVienFilter), e => e.MaUngVien.ToLower() == input.MaUngVienFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.HoVaTenFilter), e => e.HoVaTen.ToLower() == input.HoVaTenFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ViTriUngTuyenCodeFilter), e => e.ViTriUngTuyenCode.ToLower() == input.ViTriUngTuyenCodeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.KenhTuyenDungCodeFilter), e => e.KenhTuyenDungCode.ToLower() == input.KenhTuyenDungCodeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.GioiTinhCodeFilter), e => e.GioiTinhCode.ToLower() == input.GioiTinhCodeFilter.ToLower().Trim())
                        .WhereIf(input.MinNgaySinhFilter != null, e => e.NgaySinh >= input.MinNgaySinhFilter)
                        .WhereIf(input.MaxNgaySinhFilter != null, e => e.NgaySinh <= input.MaxNgaySinhFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SoCMNDFilter), e => e.SoCMND.ToLower() == input.SoCMNDFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrinhDoVanHoaFilter), e => e.TrinhDoVanHoa.ToLower() == input.TrinhDoVanHoaFilter.ToLower().Trim())
                        .WhereIf(input.MinNamTotNghiepFilter != null, e => e.NamTotNghiep >= input.MinNamTotNghiepFilter)
                        .WhereIf(input.MaxNamTotNghiepFilter != null, e => e.NamTotNghiep <= input.MaxNamTotNghiepFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TienDoTuyenDungCodeFilter), e => e.TienDoTuyenDungCode.ToLower() == input.TienDoTuyenDungCodeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RECORD_STATUSFilter), e => e.RECORD_STATUS.ToLower() == input.RECORD_STATUSFilter.ToLower().Trim())
                        .WhereIf(input.MinMARKER_IDFilter != null, e => e.MARKER_ID >= input.MinMARKER_IDFilter)
                        .WhereIf(input.MaxMARKER_IDFilter != null, e => e.MARKER_ID <= input.MaxMARKER_IDFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AUTH_STATUSFilter), e => e.AUTH_STATUS.ToLower() == input.AUTH_STATUSFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DienThoaiFilter), e => e.DienThoai.ToLower() == input.DienThoaiFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EmailFilter), e => e.Email.ToLower() == input.EmailFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DiaChiFilter), e => e.DiaChi.ToLower() == input.DiaChiFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrangThaiCodeFilter), e => e.TrangThaiCode.ToLower() == input.TrangThaiCodeFilter.Trim())
                        .WhereIf(input.MinDay1Filter != null, e => e.Day1 >= input.MinDay1Filter)
                        .WhereIf(input.MaxDay1Filter != null, e => e.Day1 <= input.MaxDay1Filter)
                           .WhereIf(input.MinDay2Filter != null, e => e.Day2 >= input.MinDay2Filter)
                        .WhereIf(input.MaxDay2Filter != null, e => e.Day2 <= input.MaxDay2Filter)
                           .WhereIf(input.MinDay3Filter != null, e => e.Day3 >= input.MinDay3Filter)
                        .WhereIf(input.MaxDay3Filter != null, e => e.Day3 <= input.MaxDay3Filter)
                          .WhereIf(!string.IsNullOrWhiteSpace(input.Time1Filter), e => e.Time1.ToLower() == input.Time1Filter.Trim())
                         .WhereIf(!string.IsNullOrWhiteSpace(input.Time2Filter), e => e.Time2.ToLower() == input.Time2Filter.Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Time3Filter), e => e.Time3.ToLower() == input.Time3Filter.Trim())
                       .WhereIf(!string.IsNullOrWhiteSpace(input.NoteFilter), e => e.Note.ToLower() == input.NoteFilter.Trim());

            var pagedAndFilteredUngViens = filteredUngViens
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var tgd = _truongGiaoDichRepository.GetAll();


            var ungViens = from o in pagedAndFilteredUngViens
                           join vtut in tgd.Where(x => x.Code == "VTUT")
                           on o.ViTriUngTuyenCode equals vtut.CDName into vtutJoin
                           from joinedvtut in vtutJoin.DefaultIfEmpty()

                           join ktd in tgd.Where(x => x.Code == "KTD") on o.KenhTuyenDungCode equals ktd.CDName into ktdJoin
                           from joinedktd in ktdJoin.DefaultIfEmpty()

                           join gt in tgd.Where(x => x.Code == "GT") on o.GioiTinhCode equals gt.CDName into gtJoin
                           from joinedgt in gtJoin.DefaultIfEmpty()

                           join tthn in tgd.Where(x => x.Code == "TTHN") on o.TinhTrangHonNhanCode equals tthn.CDName into tthnJoin
                           from joinedtthn in tthnJoin.DefaultIfEmpty()

                           join tddt in tgd.Where(x => x.Code == "TDDT") on o.TrinhDoDaoTaoCode equals tddt.CDName into tddtJoin
                           from joinedtddt in tddtJoin.DefaultIfEmpty()

                           join xl in tgd.Where(x => x.Code == "XLHL") on o.XepLoaiCode equals xl.CDName into xlJoin
                           from joinedxl in xlJoin.DefaultIfEmpty()

                           join tt in tgd.Where(x => x.Code == "TRTH") on o.TrangThaiCode equals tt.CDName into ttJoin
                           from joinedtt in ttJoin.DefaultIfEmpty()

                           join tdtd in tgd.Where(x => x.Code == "TDTD") on o.TienDoTuyenDungCode equals tdtd.CDName into tdtdJoin
                           from joinedtdtd in tdtdJoin.DefaultIfEmpty()

                           join ndt in _noiDaoTaoRepository.GetAll() on o.NoiDaoTaoID equals ndt.Id into ndtJoin
                           from joinedndt in ndtJoin.DefaultIfEmpty()

                           join tinhThanh in _tinhThanhRepository.GetAll() on o.TinhThanhID equals tinhThanh.Id into tinhThanhJoin
                           from joinedtinhThanh in tinhThanhJoin.DefaultIfEmpty()



                           select new GetUngVienForViewDto()
                           {
                               UngVien = new UngVienDto
                               {
                                   MaUngVien = o.MaUngVien,
                                   HoVaTen = o.HoVaTen,
                                   //ViTriUngTuyenCode = t.Value,
                                   ViTriUngTuyenCode = o.ViTriUngTuyenCode,
                                   KenhTuyenDungCode = o.KenhTuyenDungCode,
                                   GioiTinhCode = o.GioiTinhCode,
                                   NgaySinh = o.NgaySinh,
                                   SoCMND = o.SoCMND,
                                   NgayCap = o.NgayCap,
                                   NoiCap = o.NoiCap,
                                   TinhThanhID = o.TinhThanhID,
                                   TinhTrangHonNhanCode = o.TinhTrangHonNhanCode,
                                   TrinhDoDaoTaoCode = o.TrinhDoDaoTaoCode,
                                   TrinhDoVanHoa = o.TrinhDoVanHoa,
                                   NoiDaoTaoID = o.NoiDaoTaoID,
                                   Khoa = o.Khoa,
                                   ChuyenNganh = o.ChuyenNganh,
                                   XepLoaiCode = o.XepLoaiCode,
                                   NamTotNghiep = o.NamTotNghiep,
                                   TrangThaiCode = o.TrangThaiCode,
                                   TienDoTuyenDungCode = o.TienDoTuyenDungCode,
                                   TepDinhKem = o.TepDinhKem,
                                   RECORD_STATUS = o.RECORD_STATUS,
                                   MARKER_ID = o.MARKER_ID,
                                   AUTH_STATUS = o.AUTH_STATUS,
                                   CHECKER_ID = o.CHECKER_ID,
                                   APPROVE_DT = o.APPROVE_DT,
                                   DienThoai = o.DienThoai,
                                   Email = o.Email,
                                   DiaChi = o.DiaChi,
                                   Day1 = o.Day1,
                                   Day2 = o.Day2,
                                   Day3 = o.Day3,
                                   Time1 = o.Time1,
                                   Time2 = o.Time2,
                                   Time3 = o.Time3,
                                   Note = o.Note,
                                   Id = o.Id
                               },
                               ViTriUngTuyenValue = joinedvtut == null ? "" : joinedvtut.Value.ToString(),
                               KenhTuyenDungValue = joinedktd == null ? "" : joinedktd.Value.ToString(),
                               GioiTinhValue = joinedgt == null ? "" : joinedgt.Value.ToString(),
                               TinhTrangHonNhanValue = joinedtthn == null ? "" : joinedtthn.Value.ToString(),
                               TrinhDoDaoTaoValue = joinedtddt == null ? "" : joinedtddt.Value.ToString(),
                               XepLoaiValue = joinedxl == null ? "" : joinedxl.Value.ToString(),
                               TrangThaiValue = joinedtt == null ? "" : joinedtt.Value.ToString(),
                               TienDoTuyenDungValue = joinedtdtd == null ? "" : joinedtdtd.Value.ToString(),
                               NoiDaoTaoValue = joinedndt == null ? "" : joinedndt.TenNoiDaoTao.ToString(),
                               TinhThanhValue = joinedtinhThanh == null ? "" : joinedtinhThanh.TenTinhThanh.ToString()
                           };

            var totalCount = await filteredUngViens.CountAsync();

            return new PagedResultDto<GetUngVienForViewDto>(
                totalCount,
                await ungViens.ToListAsync()
            );
        }
        public List<UngVienDto> GetAllUngVien()
        {
           
            return ObjectMapper.Map<List<UngVienDto>>(_ungVienRepository.GetAll().ToList());
        }
        public async Task<GetUngVienForViewDto> GetUngVienForView(int id)
        {
            var ungVien = await _ungVienRepository.GetAsync(id);

            var output = new GetUngVienForViewDto { UngVien = ObjectMapper.Map<UngVienDto>(ungVien) };

            return output;
        }

        //[AbpAuthorize(AppPermissions.Pages_UngViens_Edit)]
        public async Task<GetUngVienForEditOutput> GetUngVienForEdit(EntityDto input)
        {
            var ungVien = await _ungVienRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetUngVienForEditOutput { UngVien = ObjectMapper.Map<CreateOrEditUngVienDto>(ungVien) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditUngVienDto input)
        {
            DateTime Ngay_Sinh; 
            if (input.Id == null)
            {
                if (input.SoCMND.IsNullOrEmpty())
                {
                    input.SoCMND = null;
                }
                else
                {
                    if (CheckCMND(input.SoCMND))
                    {
                        throw new UserFriendlyException("Số CMND đã bị trùng");
                       
                    }
                }
                Ngay_Sinh = Convert.ToDateTime(input.NgaySinh);
                int now = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                int dob = int.Parse(Ngay_Sinh.ToString("yyyyMMdd"));
                int age = (now - dob) / 10000;
                if(age<18)
                {
                    throw new UserFriendlyException("Chưa đủ 18 tuổi ");
                }


                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                        await conn.OpenAsync();
                    var tableName = "UngVien";
                    var result = await conn.QueryAsync<string>(sql: "exec SYS_CodeMasters_Gen " + tableName);
                    input.MaUngVien = result.ToList().First();
                    await Create(input);
                }
            }
            else
            {
                await Update(input);
            }
        }

        //[AbpAuthorize(AppPermissions.Pages_UngViens_Create)]
        protected virtual async Task Create(CreateOrEditUngVienDto input)
        {
            var ungVien = ObjectMapper.Map<UngVien>(input);

            await _ungVienRepository.InsertAsync(ungVien);
        }

        //[AbpAuthorize(AppPermissions.Pages_UngViens_Edit)]
        protected virtual async Task Update(CreateOrEditUngVienDto input)
        {
            var ungVien = await _ungVienRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, ungVien);
        }

        //[AbpAuthorize(AppPermissions.Pages_UngViens_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _ungVienRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetUngViensToExcel(GetAllUngViensForExcelInput input)
        {

            var filteredUngViens = _ungVienRepository.GetAll()
                     .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.MaUngVien.Contains(input.Filter) || e.HoVaTen.Contains(input.Filter) || e.ViTriUngTuyenCode.Contains(input.Filter) || e.KenhTuyenDungCode.Contains(input.Filter) || e.GioiTinhCode.Contains(input.Filter) || e.SoCMND.Contains(input.Filter) || e.NoiCap.Contains(input.Filter) || e.TinhTrangHonNhanCode.Contains(input.Filter) || e.TrinhDoDaoTaoCode.Contains(input.Filter) || e.TrinhDoVanHoa.Contains(input.Filter) || e.Khoa.Contains(input.Filter) || e.ChuyenNganh.Contains(input.Filter) || e.XepLoaiCode.Contains(input.Filter) || e.TrangThaiCode.Contains(input.Filter) || e.TienDoTuyenDungCode.Contains(input.Filter) || e.TepDinhKem.Contains(input.Filter) || e.RECORD_STATUS.Contains(input.Filter) || e.AUTH_STATUS.Contains(input.Filter) || e.DienThoai.Contains(input.Filter) || e.Email.Contains(input.Filter) || e.DiaChi.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MaUngVienFilter), e => e.MaUngVien.ToLower() == input.MaUngVienFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.HoVaTenFilter), e => e.HoVaTen.ToLower() == input.HoVaTenFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ViTriUngTuyenCodeFilter), e => e.ViTriUngTuyenCode.ToLower() == input.ViTriUngTuyenCodeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.KenhTuyenDungCodeFilter), e => e.KenhTuyenDungCode.ToLower() == input.KenhTuyenDungCodeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.GioiTinhCodeFilter), e => e.GioiTinhCode.ToLower() == input.GioiTinhCodeFilter.ToLower().Trim())
                        .WhereIf(input.MinNgaySinhFilter != null, e => e.NgaySinh >= input.MinNgaySinhFilter)
                        .WhereIf(input.MaxNgaySinhFilter != null, e => e.NgaySinh <= input.MaxNgaySinhFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SoCMNDFilter), e => e.SoCMND.ToLower() == input.SoCMNDFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrinhDoVanHoaFilter), e => e.TrinhDoVanHoa.ToLower() == input.TrinhDoVanHoaFilter.ToLower().Trim())
                        .WhereIf(input.MinNamTotNghiepFilter != null, e => e.NamTotNghiep >= input.MinNamTotNghiepFilter)
                        .WhereIf(input.MaxNamTotNghiepFilter != null, e => e.NamTotNghiep <= input.MaxNamTotNghiepFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TienDoTuyenDungCodeFilter), e => e.TienDoTuyenDungCode.ToLower() == input.TienDoTuyenDungCodeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RECORD_STATUSFilter), e => e.RECORD_STATUS.ToLower() == input.RECORD_STATUSFilter.ToLower().Trim())
                        .WhereIf(input.MinMARKER_IDFilter != null, e => e.MARKER_ID >= input.MinMARKER_IDFilter)
                        .WhereIf(input.MaxMARKER_IDFilter != null, e => e.MARKER_ID <= input.MaxMARKER_IDFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AUTH_STATUSFilter), e => e.AUTH_STATUS.ToLower() == input.AUTH_STATUSFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DienThoaiFilter), e => e.DienThoai.ToLower() == input.DienThoaiFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EmailFilter), e => e.Email.ToLower() == input.EmailFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DiaChiFilter), e => e.DiaChi.ToLower() == input.DiaChiFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrangThaiCodeFilter), e => e.TrangThaiCode.ToLower() == input.TrangThaiCodeFilter.Trim())
                        .WhereIf(input.MinDay1Filter != null, e => e.Day1 >= input.MinDay1Filter)
                        .WhereIf(input.MaxDay1Filter != null, e => e.Day1 <= input.MaxDay1Filter)
                           .WhereIf(input.MinDay2Filter != null, e => e.Day2 >= input.MinDay2Filter)
                        .WhereIf(input.MaxDay2Filter != null, e => e.Day2 <= input.MaxDay2Filter)
                           .WhereIf(input.MinDay3Filter != null, e => e.Day3 >= input.MinDay3Filter)
                        .WhereIf(input.MaxDay3Filter != null, e => e.Day3 <= input.MaxDay3Filter)
                          .WhereIf(!string.IsNullOrWhiteSpace(input.Time1Filter), e => e.Time1.ToLower() == input.Time1Filter.Trim())
                         .WhereIf(!string.IsNullOrWhiteSpace(input.Time2Filter), e => e.Time2.ToLower() == input.Time2Filter.Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Time3Filter), e => e.Time3.ToLower() == input.Time3Filter.Trim())
                       .WhereIf(!string.IsNullOrWhiteSpace(input.NoteFilter), e => e.Note.ToLower() == input.NoteFilter.Trim());

            var tgd = _truongGiaoDichRepository.GetAll();

            var query = (from o in filteredUngViens
                         join vtut in tgd.Where(x => x.Code == "VTUT")
                           on o.ViTriUngTuyenCode equals vtut.CDName into vtutJoin
                         from joinedvtut in vtutJoin.DefaultIfEmpty()

                         join ktd in tgd.Where(x => x.Code == "KTD") on o.KenhTuyenDungCode equals ktd.CDName into ktdJoin
                         from joinedktd in ktdJoin.DefaultIfEmpty()

                         join gt in tgd.Where(x => x.Code == "GT") on o.GioiTinhCode equals gt.CDName into gtJoin
                         from joinedgt in gtJoin.DefaultIfEmpty()

                         join tthn in tgd.Where(x => x.Code == "TTHN") on o.TinhTrangHonNhanCode equals tthn.CDName into tthnJoin
                         from joinedtthn in tthnJoin.DefaultIfEmpty()

                         join tddt in tgd.Where(x => x.Code == "TDDT") on o.TrinhDoDaoTaoCode equals tddt.CDName into tddtJoin
                         from joinedtddt in tddtJoin.DefaultIfEmpty()

                         join xl in tgd.Where(x => x.Code == "XLHL") on o.XepLoaiCode equals xl.CDName into xlJoin
                         from joinedxl in xlJoin.DefaultIfEmpty()

                         join tt in tgd.Where(x => x.Code == "TRTH") on o.TrangThaiCode equals tt.CDName into ttJoin
                         from joinedtt in ttJoin.DefaultIfEmpty()

                         join tdtd in tgd.Where(x => x.Code == "TDTD") on o.TienDoTuyenDungCode equals tdtd.CDName into tdtdJoin
                         from joinedtdtd in tdtdJoin.DefaultIfEmpty()

                         join ndt in _noiDaoTaoRepository.GetAll() on o.NoiDaoTaoID equals ndt.Id into ndtJoin
                         from joinedndt in ndtJoin.DefaultIfEmpty()

                         join tinhThanh in _tinhThanhRepository.GetAll() on o.TinhThanhID equals tinhThanh.Id into tinhThanhJoin
                         from joinedtinhThanh in tinhThanhJoin.DefaultIfEmpty()

                         select new GetUngVienForViewDto()
                         {
                             UngVien = new UngVienDto
                             {
                                 MaUngVien = o.MaUngVien,
                                 HoVaTen = o.HoVaTen,
                                 ViTriUngTuyenCode = o.ViTriUngTuyenCode,
                                 KenhTuyenDungCode = o.KenhTuyenDungCode,
                                 GioiTinhCode = o.GioiTinhCode,
                                 NgaySinh = o.NgaySinh,
                                 SoCMND = o.SoCMND,
                                 NgayCap = o.NgayCap,
                                 NoiCap = o.NoiCap,
                                 TinhThanhID = o.TinhThanhID,
                                 TinhTrangHonNhanCode = o.TinhTrangHonNhanCode,
                                 TrinhDoDaoTaoCode = o.TrinhDoDaoTaoCode,
                                 TrinhDoVanHoa = o.TrinhDoVanHoa,
                                 NoiDaoTaoID = o.NoiDaoTaoID,
                                 Khoa = o.Khoa,
                                 ChuyenNganh = o.ChuyenNganh,
                                 XepLoaiCode = o.XepLoaiCode,
                                 NamTotNghiep = o.NamTotNghiep,
                                 TrangThaiCode = o.TrangThaiCode,
                                 TienDoTuyenDungCode = o.TienDoTuyenDungCode,
                                 TepDinhKem = o.TepDinhKem,
                                 RECORD_STATUS = o.RECORD_STATUS,
                                 MARKER_ID = o.MARKER_ID,
                                 AUTH_STATUS = o.AUTH_STATUS,
                                 CHECKER_ID = o.CHECKER_ID,
                                 APPROVE_DT = o.APPROVE_DT,
                                 DienThoai = o.DienThoai,
                                 Email = o.Email,
                                 DiaChi = o.DiaChi,
                                 Day1 = o.Day1,
                                 Day2 = o.Day2,
                                 Day3 = o.Day3,
                                 Time1 = o.Time1,
                                 Time2 = o.Time2,
                                 Time3 = o.Time3,
                                 Note = o.Note,
                                 Id = o.Id
                             },
                             ViTriUngTuyenValue = joinedvtut == null ? "" : joinedvtut.Value.ToString(),
                             KenhTuyenDungValue = joinedktd == null ? "" : joinedktd.Value.ToString(),
                             GioiTinhValue = joinedgt == null ? "" : joinedgt.Value.ToString(),
                             TinhTrangHonNhanValue = joinedtthn == null ? "" : joinedtthn.Value.ToString(),
                             TrinhDoDaoTaoValue = joinedtddt == null ? "" : joinedtddt.Value.ToString(),
                             XepLoaiValue = joinedxl == null ? "" : joinedxl.Value.ToString(),
                             TrangThaiValue = joinedtt == null ? "" : joinedtt.Value.ToString(),
                             TienDoTuyenDungValue = joinedtdtd == null ? "" : joinedtdtd.Value.ToString(),
                             NoiDaoTaoValue = joinedndt == null ? "" : joinedndt.TenNoiDaoTao.ToString(),
                             TinhThanhValue = joinedtinhThanh == null ? "" : joinedtinhThanh.TenTinhThanh.ToString()
                         });


            var ungVienListDtos = await query.ToListAsync();

            return _ungViensExcelExporter.ExportToFile(ungVienListDtos);

        }

        public async Task<int> GetMaTinhThanh(string tenTP)
        {

            var TP = await _tinhThanhRepository.FirstOrDefaultAsync(x => x.TenTinhThanh == tenTP);
            return TP.Id;
        }
        public async Task<int> GetMaNoiDaotao(string tenNDT)
        {
            var NDT = await _noiDaoTaoRepository.FirstOrDefaultAsync(x => x.TenNoiDaoTao.Contains(tenNDT));
            if(NDT == null)
            {
                return 926;
            }
            return NDT.Id;
        }



        public async Task<string> importToExcel(string currentTime, string path)
        {
            var hoten = "";
            var CMND = "";
            try
            {

                string date = DateTime.Today.ToString("dd-MM-yyyy");
                SpreadsheetInfo.SetLicense("ELAP-G41W-CZA2-XNNC");
                var path1 = Path.Combine(_env.WebRootPath + "\\" + date + "\\" + currentTime + "\\" + path);
                var workbook = ExcelFile.Load(path1);

                var dataTable = new DataTable();

                dataTable.Columns.Add("Mã ứng viên", typeof(string));
                dataTable.Columns.Add("Họ và tên", typeof(string));
                dataTable.Columns.Add("Vị trí ứng tuyển", typeof(string));
                dataTable.Columns.Add("Kênh tuyển dụng", typeof(string));
                dataTable.Columns.Add("Giới tính", typeof(string));

                dataTable.Columns.Add("Ngày sinh", typeof(string));
                dataTable.Columns.Add("Số CMND", typeof(string));
                dataTable.Columns.Add("Ngày cấp", typeof(string));
                dataTable.Columns.Add("Nơi cấp", typeof(string));
                dataTable.Columns.Add("Tỉnh/Thành phố", typeof(string));

                dataTable.Columns.Add("Tình trạng hôn nhân", typeof(string));
                dataTable.Columns.Add("Trình độ đào tạo", typeof(string));
                dataTable.Columns.Add("Trình độ văn hóa", typeof(string));
                dataTable.Columns.Add("Nơi đào tạo", typeof(string));
                dataTable.Columns.Add("Khoa", typeof(string));

                dataTable.Columns.Add("Chuyên ngành", typeof(string));
                dataTable.Columns.Add("Xếp loại", typeof(string));
                dataTable.Columns.Add("Năm tốt nghiệp", typeof(string));
                dataTable.Columns.Add("Trạng thái", typeof(string));
                dataTable.Columns.Add("Tiến độ tuyển dụng", typeof(string));

                dataTable.Columns.Add("Tệp đính kèm", typeof(string));
                dataTable.Columns.Add("Record status", typeof(string));
                dataTable.Columns.Add("Marker id", typeof(string));
                dataTable.Columns.Add("Auth status", typeof(string));
                dataTable.Columns.Add("Checker id", typeof(string));

                dataTable.Columns.Add("Approve dt", typeof(string));
                dataTable.Columns.Add("Điện thoại", typeof(string));
                dataTable.Columns.Add("E-mail", typeof(string));
                dataTable.Columns.Add("Địa chỉ", typeof(string));
                dataTable.Columns.Add("Time1", typeof(string));
                dataTable.Columns.Add("Day1", typeof(string));
                dataTable.Columns.Add("Time2", typeof(string));

                dataTable.Columns.Add("Day2", typeof(string));
                dataTable.Columns.Add("Time3", typeof(string));
                dataTable.Columns.Add("Day3", typeof(string));
                dataTable.Columns.Add("Note", typeof(string));
                dataTable.Columns.Add("TenCTY", typeof(string));

                // Select the first worksheet from the file.
                var worksheet = workbook.Worksheets[0];
                var options = new ExtractToDataTableOptions(2, 0, 10000);
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
                List<string> list = new List<string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    list.Add(row[6].ToString());

                }
                List<string> duplicates = list.GroupBy(x => x)
                                                  .Where(g => g.Count() > 1)
                                                  .Select(x => x.Key).ToList();
                


                if (duplicates.Count > 0)
                {
                    foreach(var dup in duplicates)
                    {
                        CMND += dup + ", ";
                    }
                    Logger.Error("CMND:"+ CMND);
                    throw new UserFriendlyException(L("Số CMND đã bị trùng" + CMND));
                }
              
                foreach (DataRow row in dataTable.Rows)
                {
                    string NS, NC, NTN, AP, TP, NoiDT, Time1, Time2, Time3, ViTri, KenhTD, GioiTinh;
                    DateTime Ngay_Sinh, Ngay_Cap, NamTN, APPROVEDT, Day1, Day2, Day3;
                    var ungVien = new CreateOrEditUngVienDto();
                    var TGD = _truongGiaoDichRepository.GetAll();

                    if (row[1].ToString().IsNullOrEmpty())
                    {
                        throw new UserFriendlyException(L(name: "Họ tên không được để trống"));
                    }
                    else
                    {

                        ungVien.HoVaTen = row[1].ToString();
                    }

                    hoten = ungVien.HoVaTen;
                    //ungVien.ViTriUngTuyenCode = row[2].ToString().IsNullOrEmpty() ? null : TGD.FirstOrDefault(x => x.Code == "VTUT" && x.Value == row[2].ToString()).CDName;
                    ViTri = row[2].ToString();
                    if (ViTri != null)
                    {
                        //  ungVien.ViTriUngTuyenCode = TGD.FirstOrDefault(x => x.Code == "VTUT" && x.Value == ViTri).CDName;

                        var customerQuery = from tgd in TGD
                                            where tgd.Code == "VTUT" && tgd.Value == ViTri
                                            select tgd.CDName;

                        foreach (var Customer in customerQuery)
                        {
                            ungVien.ViTriUngTuyenCode = Customer;
                        }
                    }
                    else
                    {
                        ungVien.ViTriUngTuyenCode = null;
                    }
                    KenhTD = row[3].ToString();
                    if (KenhTD != null)
                    {
                        //   ungVien.KenhTuyenDungCode = TGD.FirstOrDefault(x => x.Code == "KTD" && x.Value == KenhTD).CDName;

                        var customerQuery = from tgd in TGD
                                            where tgd.Code == "KTD" && tgd.Value == KenhTD
                                            select tgd.CDName;

                        foreach (var Customer in customerQuery)
                        {
                            ungVien.KenhTuyenDungCode = Customer;
                        }
                    }
                    else
                    {
                        ungVien.KenhTuyenDungCode = null;
                    }
                    GioiTinh = row[4].ToString();
                    if (GioiTinh != null)

                    {
                        //  ungVien.GioiTinhCode = TGD.FirstOrDefault(x => x.Code == "GT" && x.Value == GioiTinh).CDName;
                        var customerQuery = from tgd in TGD
                                            where tgd.Code == "GT" && tgd.Value == GioiTinh
                                            select tgd.CDName;

                        foreach (var Customer in customerQuery)
                        {
                            ungVien.GioiTinhCode = Customer;
                        }
                    }
                    else
                    {
                        ungVien.GioiTinhCode = null;
                    }

                    NS = row[5].ToString();
                    if (NS != "")
                    {
                        try
                        {
                            Ngay_Sinh = Convert.ToDateTime(NS);
                            ungVien.NgaySinh = Ngay_Sinh;

                        }
                        catch(Exception ex)
                        {
                            ungVien.NgaySinh = DateTime.MinValue;
                        }
                        
                    }
                    else
                    {
                        ungVien.NgaySinh = null;
                    }
                 
                    ungVien.SoCMND = row[6].ToString();
                    //if (CheckCMND(cmnd) && !cmnd.IsNullOrWhiteSpace())
                    //{
                    //    throw new UserFriendlyException(L("Số CMND đã bị trùng"));
                    //}
                    //else
                    //{





                    //   List<string> AuthorList = new List<string>();
                    //   //AuthorList.Add(cmnd);
                    //if(AuthorList.Count> 0 )
                    //   {
                    //       foreach (var author in AuthorList)
                    //       {
                    //           if (author == cmnd)
                    //           {
                    //               throw new UserFriendlyException(L("Số CMND đã bị trùng"));
                    //           }
                    //           else
                    //           {
                    //               ungVien.SoCMND = cmnd;
                    //           }
                    //       }
                    //       AuthorList.Add(cmnd);
                    //   }
                    //else
                    //   {

                    //   }





                    NC = row[7].ToString();
                    if (NC != "")
                    {

                        Ngay_Cap = Convert.ToDateTime(NC);
                        ungVien.NgayCap = Ngay_Cap;
                    }
                    else
                    {
                        ungVien.NgayCap = null;
                    }

                    ungVien.NoiCap = row[8].ToString();
                    TP = row[9].ToString();
                    if (TP == "")
                    {
                        ungVien.TinhThanhID = null;
                    }

                    else
                    {
                        int id = await GetMaTinhThanh(TP);
                        ungVien.TinhThanhID = id;

                    }



                    string TinhTrangHonNhanCode = row[10].ToString();
                    if (TinhTrangHonNhanCode != null)
                    {
                        // ungVien.TinhTrangHonNhanCode = TGD.FirstOrDefault(x => x.Code == "TTHN" && x.Value == TinhTrangHonNhanCode).CDName;
                        var customerQuery = from tgd in TGD
                                            where tgd.Code == "TTHN" && tgd.Value == TinhTrangHonNhanCode
                                            select tgd.CDName;

                        foreach (var Customer in customerQuery)
                        {
                            ungVien.TinhTrangHonNhanCode = Customer;
                        }
                    }
                    else
                    {
                        ungVien.TinhTrangHonNhanCode = null;
                    }

                    string TrinhDoDaoTaoCode = row[11].ToString();
                    if (TrinhDoDaoTaoCode != null)
                    {
                        //   ungVien.TrinhDoDaoTaoCode = TGD.FirstOrDefault(x => x.Code == "TDDT" && x.Value == TrinhDoDaoTaoCode).CDName;
                        var customerQuery = from tgd in TGD
                                            where tgd.Code == "TDDT" && tgd.Value == TrinhDoDaoTaoCode
                                            select tgd.Code;

                        foreach (var Customer in customerQuery)
                        {
                            ungVien.TrinhDoDaoTaoCode = Customer;
                        }
                    }
                    else
                    {
                        ungVien.TrinhDoDaoTaoCode = null;
                    }
                    ungVien.TrinhDoVanHoa = row[12].ToString();
                    NoiDT = row[13].ToString();
                    if (NoiDT != "")
                    {
                        int id = await GetMaNoiDaotao(NoiDT);
                        ungVien.NoiDaoTaoID = id;
                    }
                    else
                    {
                        ungVien.NoiDaoTaoID = null;
                    }

                    ungVien.Khoa = row[14].ToString();

                    ungVien.ChuyenNganh = row[15].ToString();


                    string XepLoaiCode = row[16].ToString();
                    if (XepLoaiCode != null)
                    {

                        // ungVien.XepLoaiCode = TGD.FirstOrDefault(x => x.Code == "XLHL" && x.Value == XepLoaiCode).CDName;
                        var customerQuery = from tgd in TGD
                                            where tgd.Code == "XLHL" && tgd.Value == XepLoaiCode
                                            select tgd.CDName;

                        foreach (var Customer in customerQuery)
                        {
                            ungVien.XepLoaiCode = Customer;
                        }
                    }
                    else
                    {
                        ungVien.XepLoaiCode = null;
                    }

                    NTN = row[17].ToString();
                    if (NTN != "")
                    {

                        ungVien.NamTotNghiep = int.Parse((NTN).ToString());
                    }
                    else
                    {
                        ungVien.NamTotNghiep = null;
                    }

                    string TienDoTuyenDungCode = row[19].ToString();
                    if (TienDoTuyenDungCode != null)
                    {
                        //  ungVien.TienDoTuyenDungCode = TGD.FirstOrDefault(x => x.Code == "TDTD" && x.Value == TienDoTuyenDungCode).CDName;
                        var customerQuery = from tgd in TGD
                                            where tgd.Code == "TDTD" && tgd.Value == TienDoTuyenDungCode
                                            select tgd.CDName;

                        foreach (var Customer in customerQuery)
                        {
                            ungVien.TienDoTuyenDungCode = Customer;
                        }
                    }
                    else
                    {
                        ungVien.TienDoTuyenDungCode = null;
                    }

                    string TrangThaiCode = row[18].ToString();
                    if (TrangThaiCode != null)
                    {
                        //  ungVien.TrangThaiCode = TGD.FirstOrDefault(x => x.Code == "TRTH" && x.Value == TrangThaiCode).CDName;
                        var customerQuery = from tgd in TGD
                                            where tgd.Code == "TRTH" && tgd.Value == TrangThaiCode
                                            select tgd.CDName;

                        foreach (var Customer in customerQuery)
                        {
                            ungVien.TrangThaiCode = Customer;
                        }
                    }
                    else
                    {
                        ungVien.TrangThaiCode = null;
                    }
                    ungVien.TepDinhKem = row[20].ToString();
                    ungVien.RECORD_STATUS = row[21].ToString();
                    if (row[22].ToString() != "")
                    {
                        ungVien.MARKER_ID = int.Parse(row[22].ToString());
                    }
                    else
                    {
                        ungVien.MARKER_ID = null;
                    }
                    ungVien.AUTH_STATUS = row[23].ToString();
                    if (row[24].ToString() != "")
                    {
                        ungVien.CHECKER_ID = int.Parse(row[24].ToString());
                    }
                    else
                    {
                        ungVien.CHECKER_ID = null;
                    }

                    AP = row[25].ToString();
                    if (AP != "")
                    {
                        APPROVEDT = Convert.ToDateTime(AP);
                        ungVien.APPROVE_DT = APPROVEDT;
                    }
                    else
                    {
                        ungVien.APPROVE_DT = null;
                    }

                    ungVien.DienThoai = row[26].ToString();
                    ungVien.Email = row[27].ToString();
                    ungVien.DiaChi = row[28].ToString();

                    ungVien.Time1 = row[29].ToString();

                    string day1 = row[30].ToString();
                    if (day1 != "")
                    {
                        Day1 = Convert.ToDateTime(day1);
                        ungVien.Day1 = Day1;
                    }
                    else
                    {
                        ungVien.Day1 = null;
                    }
                    ungVien.Time2 = row[31].ToString();

                    string day2 = row[32].ToString();
                    if (day2 != "")
                    {
                        Day2 = Convert.ToDateTime(day2);
                        ungVien.Day2 = Day2;
                    }
                    else
                    {
                        ungVien.Day2 = null;
                    }
                    ungVien.Time3 = row[33].ToString();
                    string day3 = row[34].ToString();
                    if (day3 != "")
                    {
                        Day3 = Convert.ToDateTime(day3);
                        ungVien.Day3 = Day3;
                    }
                    else
                    {
                        ungVien.Day3 = null;
                    }

                    ungVien.Note = row[35].ToString();
                    ungVien.TenCTY = row[36].ToString();
                    //  var UngViens = ObjectMapper.Map<UngVien>(ungVien);
                    await CreateOrEdit(ungVien);

                   // await _ungVienRepository.InsertAsync(UngViens);


                }

                return mes;
            }

            catch (Exception ex) 
            {
                //throw new UserFriendlyException(L("Số CMND đã bị trùng" + CMND));
                Logger.Error("CMND:"+ CMND);
                Logger.Error(ex.StackTrace);
                return "";
            }
        }

    }

}
