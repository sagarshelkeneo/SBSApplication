using Client.Application.Features.SubContractor.Dtos;
using Client.Application.Interfaces;
using NuGet.Protocol.Core.Types;

namespace Client_WebApp.Services.Master
{
    public class SubContractorService
    {
        private readonly ISubContractorRepository _repository;

        public SubContractorService(ISubContractorRepository repository)
        {
            _repository = repository;
        }

        public Task<List<SubContractorDto>> GetAllSubContractorAsync(int companyId, int? id = null)
        {
            string? search = null;
            return _repository.GetSubContractorsAsync(id, search, companyId);
        }

        public Task<List<SubContractorDto>> CreateSubContractorAsync(CreateSubContractorDto dto)
        {
            return _repository.CreateSubContractorAsync(dto);
        }

        public Task<List<SubContractorDto>> UpdateSubContractorAsync(UpdateSubContractorDto dto)
        {
            return _repository.UpdateSubContractorAsync(dto);
        }

        public Task<List<SubContractorDto>> DeleteSubContractorAsync(int id, int updatedBy, int companyId)
        {
            return _repository.DeleteSubContractorAsync(id, updatedBy, companyId);
        }
    }
}
