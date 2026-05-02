using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.Invoice.Dtos
{
    public class CreateInvoiceDto
    {
        public string InvoiceNo { get; set; }
        public int CompanyId { get; set; }
        public int SubcontractorId { get; set; }
        public int ProductId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CommissionPercentage { get; set; }
        public decimal CommissionAmount { get; set; }
        public string? Status { get; set; }
        public string PaymentMode { get; set; }
        public int CreatedBy { get; set; }
        public string? GroupNumber { get; set; }
        public string? LRNumber { get; set; }
        public string? VehicleNumber { get; set; }
        public bool IsLeviApplicable { get; set; }
        public string? Levi { get; set; }
        public string? DocketNumber { get; set; }
        public int? TrollyQuantity { get; set; }
        public decimal? TrollyAmount { get; set; }
    }
}
