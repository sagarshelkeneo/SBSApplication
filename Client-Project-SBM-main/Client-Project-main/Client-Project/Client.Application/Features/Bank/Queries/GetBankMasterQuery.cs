using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Bank.Dtos;
using MediatR;

namespace Client.Application.Features.Bank.Queries
{
    public class GetBankMasterQuery : IRequest<List<BankMasterDto>>
    {
        public int? Id { get; }

        public GetBankMasterQuery(int? id)
        {
            Id = id;
        }
    }
}
