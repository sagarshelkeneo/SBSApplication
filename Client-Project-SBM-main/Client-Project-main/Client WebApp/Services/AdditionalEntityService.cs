using Client.Application.Features.AdditionalEntity.Dtos;
using Client.Application.Interfaces;

namespace Client_WebApp.Services
{
    public class AdditionalEntityService
    {
        private readonly IAdditionalEntityRepository _repository;

        public AdditionalEntityService(IAdditionalEntityRepository repository)
        {
            _repository = repository;
        }

        public Task<List<AdditionalEntityDto>> GetAllAsync(int companyId, int? id = null)
        {
            return _repository.GetAsync(companyId, id);
        }

        public Task<List<AdditionalEntityDto>> InsertAsync(CreateAdditionalEntityDto dto)
        {
            return _repository.InsertAsync(dto);
        }

        public Task<List<AdditionalEntityDto>> UpdateAsync(UpdateAdditionalEntityDto dto)
        {
            return _repository.UpdateAsync(dto);
        }

        public Task<List<AdditionalEntityDto>> DeleteAsync(int id, int updatedBy, int companyId)
        {
            return _repository.DeleteAsync(id, updatedBy, companyId);
        }
    }
}
