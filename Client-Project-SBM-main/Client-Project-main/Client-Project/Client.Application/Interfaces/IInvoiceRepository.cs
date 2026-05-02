using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Invoice.Dtos;

namespace Client.Application.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<List<InvoiceDetailsDto>> CreateInvoiceAsync(CreateInvoiceDto dto);
        Task<List<InvoiceDetailsDto>> UpdateInvoiceAsync(UpdateInvoiceDto dto);
        Task<List<InvoiceDetailsDto>> DeleteInvoiceAsync(int id, int updatedBy,int companyId, bool isLeviApplicable);


        Task<List<InvoiceDetailsDto>> GetInvoicesAsync(bool IsLeviApplicable, int companyId,int? id = null);
    }

}
