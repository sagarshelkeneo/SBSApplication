using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Invoice.Dtos;
using Client.Application.Features.Invoice.Queries;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Invoice.Handlers
{
    public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, List<InvoiceDetailsDto>>
    {
        private readonly IInvoiceRepository _repo;

        public GetInvoicesQueryHandler(IInvoiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<InvoiceDetailsDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetInvoicesAsync(request.CompanyId,request.Id);
        }
    }

}
