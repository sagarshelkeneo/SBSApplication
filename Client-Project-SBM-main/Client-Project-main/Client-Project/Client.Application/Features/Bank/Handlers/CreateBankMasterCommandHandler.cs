using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Bank.Commands;
using Client.Application.Features.Bank.Dtos;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Bank.Handlers
{

    public class CreateBankMasterCommandHandler : IRequestHandler<CreateBankMasterCommand, List<BankMasterDto>>
    {
        private readonly IBankMasterRepository _repository;

        public CreateBankMasterCommandHandler(IBankMasterRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<BankMasterDto>> Handle(CreateBankMasterCommand request, CancellationToken cancellationToken)
        {
            return await _repository.CreateBankAsync(request.Dto);
        }
    }

}
