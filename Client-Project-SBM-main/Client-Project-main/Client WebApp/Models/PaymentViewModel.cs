using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Client_WebApp.Models
{
    public class PaymentDetails
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentMode { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class AddPaymentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Invoice No is required")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Payment Date")]
        public DateTime? PaymentDate { get; set; } = DateTime.Today;

        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }

        [Required(ErrorMessage = "Amount Paid is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Display(Name = "Amount Paid")]
        public decimal AmountPaid { get; set; }

        [Required(ErrorMessage = "Payment Mode is required")]
        //[Display(Name = "Payment Mode")]
        //public string PaymentMode { get; set; } // CASH or BALANCE

        public int? BankId { get; set; }

        [Display(Name = "Bank Name")]
        public string? BankName { get; set; }

        //[Display(Name = "Status")]
        //public string PaymentStatus { get; set; } // Paid / Pending
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        [Display(Name = "Payment Duration")]
        public string ModalDurationType { get; set; } = "day"; // default
        public IEnumerable<SelectListItem>? Invoices { get; set; }
        public IEnumerable<SelectListItem>? Bankes { get; set; }
    }


    public class PaymentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Invoice No is required")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Payment Date")]
        public DateTime? PaymentDate { get; set; }

        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }

        [Required(ErrorMessage = "Amount Paid is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Display(Name = "Amount Paid")]
        public decimal AmountPaid { get; set; }

        [Required(ErrorMessage = "Payment Mode is required")]
        [Display(Name = "Payment Mode")]
        public string PaymentMode { get; set; } // CASH or BALANCE

        [Display(Name = "Bank Name")]
        public int? BankId { get; set; }
        public string? BankName { get; set; }

        [Display(Name = "Status")]
        public string PaymentStatus { get; set; } // Paid / Pending
        public string DurationType { get; set; } = "day";
        // Audit fields
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class PaymentIndexViewModel
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }

        // Filters
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string BankName { get; set; }

        public PaymentViewModel PaymentViewModel { get; set; } = new PaymentViewModel();
        public AddPaymentViewModel AddPaymentViewModel { get; set; } = new AddPaymentViewModel();
        public IEnumerable<SelectListItem> Invoices { get; set; }
        public IEnumerable<SelectListItem> Bankes { get; set; }
        // Payment data
        public List<PaymentViewModel> Payments { get; set; } = new List<PaymentViewModel>();

        // Modal dropdown data
        public List<BankViewModel> Banks { get; set; } = new List<BankViewModel>();
    }

    public class BankViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
