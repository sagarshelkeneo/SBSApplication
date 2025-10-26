using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Invoice.Dtos;
using MediatR;

namespace Client.Application.Features.Invoice.Queries
{
    public class GetInvoicesQuery : IRequest<List<InvoiceDetailsDto>>
    {
        public int CompanyId { get; set; }
        public int? Id { get; }

        public GetInvoicesQuery(int companyId,int? id = null)
        {
            CompanyId = companyId;
            Id = id;
        }
    }

}
