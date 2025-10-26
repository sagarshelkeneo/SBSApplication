using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Bank.Dtos;

namespace Client.Application.Interfaces
{
    public interface IBankMasterRepository
    {
        Task<List<BankMasterDto>> CreateBankAsync(CreateBankMasterDto dto);
        Task<List<BankMasterDto>> UpdateBankAsync(UpdateBankMasterDto dto);
        Task<List<BankMasterDto>> DeleteBankAsync(DeleteBankMasterDto dto);
        Task<List<BankMasterDto>> GetBanksAsync(int? id);
    }


}
