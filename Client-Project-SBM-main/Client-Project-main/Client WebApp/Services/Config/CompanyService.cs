using Client.Application.Features.Company.Dtos;
using Client.Application.Features.Payment.Dtos;
using Client.Application.Features.Product.Dtos;
using Client.Application.Interfaces;

namespace Client_WebApp.Services.Config
{
    public class CompanyService
    {

        private readonly ICompanyRepository _repository;

        public CompanyService(ICompanyRepository repository)
        {
            _repository = repository;
        }

        public Task<List<CompanyDto>> GetCompanyAsync(int? companyId, string? search = null)
        {
            return _repository.GetCompaniesAsync(companyId, search);
        }

        public Task<List<CompanyDto>> CreateCompanyAsync(CreateCompanyDto dto)
        {
            return _repository.CreateCompanyAsync(dto);
        }

        public Task<List<CompanyDto>> UpdateCompanyAsync(UpdateCompanyDto dto)
        {
            return _repository.UpdateCompanyAsync(dto);
        }

        public Task<List<CompanyDto>> DeleteCompanyAsync(int id, int updatedBy, int companyId)
        {
            return _repository.DeleteCompanyAsync(id, updatedBy);
        }
    }
}
