using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Client.Domain.Models
{
    public class InvoiceDetails
    {
        [Key]
        public int Id { get; set; }
        [NotNull]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        [NotNull]
        public int SubcontractorId { get; set; }
        public string SubcontractorName { get; set; }
        [NotNull]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        [NotNull]
        public DateTime InvoiceDate { get; set; }
        [NotNull, MaxLength(20, ErrorMessage = "Status length exceeding max length 20")]
        public string Status { get; set; } = "Pending";
        [NotNull]
        public int Quantity { get; set; }
        [NotNull]
        public decimal TotalAmount { get; set; } = 0;
        [NotNull]
        public string PaymentMode { get; set; }
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
