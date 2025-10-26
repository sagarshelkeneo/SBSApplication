using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Client.Domain.Models
{
    public class PaymentDetails
    {
        [Key]
        public int Id { get; set; }
        [NotNull]
        public int InvoiceId { get; set; }
        [NotNull]
        public DateTime PaymentDate { get; set; }
        [NotNull]
        public decimal AmountPaid { get; set; }
        [NotNull, MaxLength(20, ErrorMessage = "PaymentMode length exceeding max length 20")]
        public string PaymentMode { get; set; }
        [NotNull, MaxLength(255, ErrorMessage = "BankName length exceeding max length 255")]
        public string BankName { get; set; }
        [NotNull, MaxLength(20, ErrorMessage = "PaymentStatus length exceeding max length 20")]
        public string PaymentStatus { get; set; } = "Pending";
        [NotNull]
        public int CreatedBy { get; set; }
        [NotNull]
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [NotNull]
        public int IsActive { get; set; } = 1;
        [NotNull]
        public int IsDeleted { get; set; } = 0;
    }
}
