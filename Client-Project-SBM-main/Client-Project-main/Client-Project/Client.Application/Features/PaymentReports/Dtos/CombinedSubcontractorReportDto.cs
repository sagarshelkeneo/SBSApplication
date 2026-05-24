using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.PaymentReports.Dtos
{

    public class CombinedSubcontractorReportDto
    {
        public string Date { get; set; }
        public string DocketNumber { get; set; }
        public string InvoiceNo { get; set; }
        public string Customer { get; set; }
        public string? Quantity { get; set; }
        public string? TotalAmount { get; set; }
        public string? LevhiAmount { get; set; }
        public string? FinalAmount { get; set; }
    }
}
