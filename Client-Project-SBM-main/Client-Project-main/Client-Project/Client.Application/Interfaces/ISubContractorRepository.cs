using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.SubContractor.Dtos;

namespace Client.Application.Interfaces
{
    public interface ISubContractorRepository
    {
        Task<List<SubContractorDto>> GetSubContractorsAsync(int? id, string? search,int companyId);
        Task<List<SubContractorDto>> CreateSubContractorAsync(CreateSubContractorDto dto);
        Task<List<SubContractorDto>> UpdateSubContractorAsync(UpdateSubContractorDto dto);
        Task<List<SubContractorDto>> DeleteSubContractorAsync(int id,int updatedBy, int companyId);

    }

}
