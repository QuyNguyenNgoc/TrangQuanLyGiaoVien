using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Hinnova.QLNSDtos;
using Hinnova.Dto;
using System.Collections.Generic;
using Abp.Organizations;
using System.Linq;

namespace Hinnova.QLNS
{
    public interface IHoSosAppService : IApplicationService 
    {
        //Task<PagedResultDto<GetHoSoForViewDto>> GetAll(GetAllHoSosInput input);

        Task<CreateOrEditHoSoDto> GetHoSoForView(int id);
        Task<List<string>> GetNameUnit(int id);
        IQueryable<OrganizationUnit> GetAllCMND1();
        Task<GetHoSoForEditOutput> GetHoSoForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditHoSoDto input);

		Task Delete(EntityDto input);

		//Task<FileDto> GetHoSosToExcel(GetAllHoSosForExcelInput input);

		
    }
}