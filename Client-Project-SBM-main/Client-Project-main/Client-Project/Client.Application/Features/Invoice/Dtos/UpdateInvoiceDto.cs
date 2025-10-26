using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.Invoice.Dtos
{
    public class UpdateInvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; }
        public int? ProductId { get; set; }
        public int? SubcontractorId { get; set; }
        public int CompanyId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? CommissionPercentage { get; set; }
        public decimal? CommissionAmount { get; set; }
        public string? PaymentMode { get; set; }
        public string? Status { get; set; }
        public int UpdatedBy { get; set; }
    }

}
