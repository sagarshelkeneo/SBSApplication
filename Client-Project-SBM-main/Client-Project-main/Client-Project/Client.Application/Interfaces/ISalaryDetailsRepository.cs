using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.SalaryDetails.Dtos;

namespace Client.Application.Interfaces
{
    public interface ISalaryDetailsRepository
    {
        Task<List<SalaryDetailsDto>> InsertAsync(CreateSalaryDetailsDto dto);
        Task<List<SalaryDetailsDto>> UpdateAsync(UpdateSalaryDetailsDto dto);
        Task<List<SalaryDetailsDto>> DeleteAsync(int id, int updatedBy, int companyId);
        Task<List<SalaryDetailsDto>> GetAsync(int companyId, int? id = null, int? subContractorId = null);
    }
}
