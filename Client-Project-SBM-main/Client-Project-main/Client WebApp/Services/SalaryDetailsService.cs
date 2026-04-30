using Client.Application.Features.SalaryDetails.Dtos;
using Client.Application.Interfaces;

namespace Client_WebApp.Services
{
    public class SalaryDetailsService
    {
        private readonly ISalaryDetailsRepository _repository;

        public SalaryDetailsService(ISalaryDetailsRepository repository)
        {
            _repository = repository;
        }

        public Task<List<SalaryDetailsDto>> GetAllAsync(int companyId, int? id = null)
        {
            return _repository.GetAsync(companyId, id);
        }

        public Task<List<SalaryDetailsDto>> InsertAsync(CreateSalaryDetailsDto dto)
        {
            return _repository.InsertAsync(dto);
        }

        public Task<List<SalaryDetailsDto>> UpdateAsync(UpdateSalaryDetailsDto dto)
        {
            return _repository.UpdateAsync(dto);
        }

        public Task<List<SalaryDetailsDto>> DeleteAsync(int id, int updatedBy, int companyId)
        {
            return _repository.DeleteAsync(id, updatedBy, companyId);
        }
    }
}
