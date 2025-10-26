using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.Bank.Dtos
{
    public class UpdateBankMasterDto
    {
        public int Id { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public int UpdatedBy { get; set; }
    }
}
