using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.PaymentReports.Dtos
{

    public class UnpaidReportDto
    {

        public string? SubContractor { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? InvoiceAmount { get; set; }
        
        public string? BankName { get; set; }
        public string? ProductName { get; set; }
        public string? ReceiptNumber { get; set; }
        public string? LRNumber { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal PaidAmount { get; set; }
        public string? TotalTypeRate { get; set; }
        public string? Quantity { get; set; }
        public string? UnitAmount { get; set; }
        public string? CommissionAmount { get; set; }
        public decimal Bank{ get; set; }
        public decimal Cash { get; set; }
        public decimal Balance { get; set; }

    }

}
