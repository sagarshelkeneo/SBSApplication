using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.Payment.Dtos
{
    public class CreatePaymentDto
    {
        public string? InvoiceNo { get; set; }
        public int CompanyId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal AmountPaid { get; set; }
        public string? PaymentMode { get; set; }
        public int? BankId { get; set; }
        public string? PaymentStatus { get; set; }
        public int CreatedBy { get; set; }
        public string DurationType { get; set; }
    }

}
