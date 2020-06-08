using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Hinnova.QLNSDtos;
using Hinnova.Dto;

namespace Hinnova.QLNS
{
    public interface IHopDongsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetHopDongForViewDto>> GetAll(GetAllHopDongsInput input);

        Task<GetHopDongForViewDto> GetHopDongForView(int id);

		Task<GetHopDongForEditOutput> GetHopDongForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditHopDongDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetHopDongsToExcel(GetAllHopDongsForExcelInput input);

		
    }
}