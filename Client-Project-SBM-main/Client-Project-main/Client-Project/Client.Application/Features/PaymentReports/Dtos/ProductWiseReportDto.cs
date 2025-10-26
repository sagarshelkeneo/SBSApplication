using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.PaymentReports.Dtos
{
    public class ProductWiseReportDto
    {
        public string ProdName { get; set; }
        public string SubContractorName { get; set; }
        public decimal CashAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal PaidAmount { get; set; }
    }

}
