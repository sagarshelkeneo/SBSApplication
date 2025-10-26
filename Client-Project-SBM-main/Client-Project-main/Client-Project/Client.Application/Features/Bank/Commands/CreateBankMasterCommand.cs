using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Bank.Dtos;
using MediatR;

namespace Client.Application.Features.Bank.Commands
{
    public class CreateBankMasterCommand : IRequest<List<BankMasterDto>>
    {
        public CreateBankMasterDto Dto { get; }

        public CreateBankMasterCommand(CreateBankMasterDto dto)
        {
            Dto = dto;
        }
    }
}
