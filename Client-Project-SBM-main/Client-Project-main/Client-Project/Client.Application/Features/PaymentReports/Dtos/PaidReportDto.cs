using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.PaymentReports.Dtos
{
    public class PaidReportDto
    {

        public string? SubContractor { get; set; }
        public string? BankName { get; set; }
        public string? ReceiptNumber { get; set; }
        public string? LRNumber { get; set; }
        public string? VehicleNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string? TotalTypeRate { get; set; }
        public string? Box { get; set; }
        public string? Peti { get; set; }
        public string? Motors { get; set; }
    }

}
