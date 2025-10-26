using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.Bank.Dtos
{
    public class CreateBankMasterDto
    {
        public string BankName { get; set; }
        public string Branch { get; set; }
        public int CreatedBy { get; set; }
    }
}
