using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Bank.Dtos;
using MediatR;

namespace Client.Application.Features.Bank.Commands
{
    public class UpdateBankMasterCommand : IRequest<List<BankMasterDto>>
    {
        public UpdateBankMasterDto Dto { get; }

        public UpdateBankMasterCommand(UpdateBankMasterDto dto)
        {
            Dto = dto;
        }
    }

}
