using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Bank.Dtos;
using MediatR;

namespace Client.Application.Features.Bank.Commands
{
    public class DeleteBankMasterCommand : IRequest<List<BankMasterDto>>
    {
        public DeleteBankMasterDto Dto { get; }

        public DeleteBankMasterCommand(DeleteBankMasterDto dto)
        {
            Dto = dto;
        }
    }

}
