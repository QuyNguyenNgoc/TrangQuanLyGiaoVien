using Abp.Application.Services.Dto;
using System;

namespace Hinnova.QLVB.Dtos
{
    public class GetAllMemorizeKeywordsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string KeyWordFilter { get; set; }



    }
}