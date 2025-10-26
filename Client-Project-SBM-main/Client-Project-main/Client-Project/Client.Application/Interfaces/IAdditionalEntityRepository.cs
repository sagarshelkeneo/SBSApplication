using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.AdditionalEntity.Dtos;

namespace Client.Application.Interfaces
{
    public interface IAdditionalEntityRepository
    {
        Task<List<AdditionalEntityDto>> InsertAsync(CreateAdditionalEntityDto dto);
        Task<List<AdditionalEntityDto>> UpdateAsync(UpdateAdditionalEntityDto dto);
        Task<List<AdditionalEntityDto>> DeleteAsync(int id, int updatedBy, int companyId);
        Task<List<AdditionalEntityDto>> GetAsync(int companyId, int? id = null, int? subContractorId = null);
    }
}
