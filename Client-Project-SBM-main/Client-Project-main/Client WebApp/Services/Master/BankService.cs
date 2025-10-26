using Client.Application.Features.Bank.Dtos;
using Client.Application.Interfaces;

namespace Client_WebApp.Services.Master
{
    public class BankService
    {
        private readonly IBankMasterRepository _repository;

        public BankService(IBankMasterRepository repository)
        {
            _repository = repository;
        }

        // Get all banks or by ID
        public Task<List<BankMasterDto>> GetAllBanksAsync(int? id = null)
        {
            return _repository.GetBanksAsync(id);
        }

        // Create bank
        public Task<List<BankMasterDto>> CreateBankAsync(CreateBankMasterDto dto)
        {
            return _repository.CreateBankAsync(dto);
        }

        // Update bank
        public Task<List<BankMasterDto>> UpdateBankAsync(UpdateBankMasterDto dto)
        {
            return _repository.UpdateBankAsync(dto);
        }

        // Delete bank
        public Task<List<BankMasterDto>> DeleteBankAsync(int id, int updatedBy)
        {
            var deleteDto = new DeleteBankMasterDto
            {
                Id = id,
                UpdatedBy = updatedBy
            };
            return _repository.DeleteBankAsync(deleteDto);
        }
    }
}
