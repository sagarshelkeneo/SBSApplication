using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.Payment.Dtos
{
    public class PaymentDetailsDto
    {
        public int R_id { get; set; }
        public string R_invoiceNo { get; set; }
        public DateTime? R_paymentDate { get; set; }
        public DateTime? R_fromDate { get; set; }
        public DateTime? R_toDate { get; set; }
        public decimal R_amountPaid { get; set; }
        public string R_paymentMode { get; set; }
        public int R_bankId { get; set; }
        public string R_bankName { get; set; }
        public string R_paymentStatus { get; set; }
    }

}
