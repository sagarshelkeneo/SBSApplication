using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Invoice.Dtos;
using MediatR;

namespace Client.Application.Features.Invoice.Commands
{
    public record UpdateInvoiceCommand(UpdateInvoiceDto Invoice) : IRequest<List<InvoiceDetailsDto>>;

}
