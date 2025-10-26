using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Bank.Dtos;
using Client.Application.Features.Bank.Queries;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Bank.Handlers
{
    public class GetBankMasterQueryHandler : IRequestHandler<GetBankMasterQuery, List<BankMasterDto>>
    {
        private readonly IBankMasterRepository _repository;

        public GetBankMasterQueryHandler(IBankMasterRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<BankMasterDto>> Handle(GetBankMasterQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetBanksAsync(request.Id);
        }
    }
}
