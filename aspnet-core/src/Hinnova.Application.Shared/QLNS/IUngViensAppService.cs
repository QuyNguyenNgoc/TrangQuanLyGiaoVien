using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Hinnova.QLNSDtos;
using Hinnova.Dto;

namespace Hinnova.QLNS
{
    public interface IUngViensAppService : IApplicationService 
    {
        Task<string> importToExcel(string currentTime, string path);
        Task<PagedResultDto<GetUngVienForViewDto>> GetAll(GetAllUngViensInput input);

        Task<GetUngVienForViewDto> GetUngVienForView(int id);

		Task<GetUngVienForEditOutput> GetUngVienForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditUngVienDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetUngViensToExcel(GetAllUngViensForExcelInput input);

		
    }
}