using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Client_WebApp.Models
{
    public class InvoiceIndexViewModel
    {
        public InvoiceViewModel NewInvoice { get; set; } = new InvoiceViewModel();
        public AddInvoiceViewModel AddInvoice { get; set; } = new AddInvoiceViewModel();
        public List<InvoiceDetailsDto> Invoices { get; set; } = new List<InvoiceDetailsDto>();
    }

    public class InvoiceDetailsDto
    {
        public int Id { get; set; }
        public string? InvoiceNo { get; set; } = null;
        public int CompanyId { get; set; }
        public int SubContractorId { get; set; }
        public string SubContractorName { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CommissionPercentage { get; set; }
        public decimal CommissionAmount { get; set; }
        public string InvoiceType { get; set; }
        public string GroupNumber { get; set; }
        public string LRNumber { get; set; }
        public string VehicleNumber { get; set; }
        public bool IsLeviApplicable { get; set; }
        public string Levi { get; set; }
        public string DocketNumber { get; set; }
        public int TrollyQuantity { get; set; }
        public decimal TrollyAmount { get; set; }
    }

    public class InvoiceViewModel
    {
        public int Id { get; set; }

        public string? InvoiceNo { get; set; } = null;

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public int SubContractorId { get; set; }

        public string SubContractorName { get; set; }
        public string ProductName { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public decimal UnitAmount { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public decimal CommissionPercentage { get; set; }
        public decimal CommissionAmount { get; set; }

        public string? PaymentMode { get; set; }
        public string? GroupNumber { get; set; }
        public string? LRNumber { get; set; }
        public string? VehicleNumber { get; set; }

        public bool IsLeviApplicable { get; set; }
        public string? Levi { get; set; }
        public string? DocketNumber { get; set; }
        public int? TrollyQuantity { get; set; }
        public decimal? TrollyAmount { get; set; }

        public string RecaptchaToken { get; set; }
        
        // Dropdown lists
        public IEnumerable<SelectListItem> SubContractorList { get; set; }
        public IEnumerable<SelectListItem> ProductList { get; set; }
        public string InvoiceType { get; set; } = "CASH";
    }

    public class AddInvoiceViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Invoice No")]
        public string? InvoiceNo { get; set; } = null;
        public int CompanyId { get; set; }
        [Display(Name = "Sub Contractor")]
        public int SubcontractorId { get; set; }
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Amount")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Amount must be greater than 0")]
        public decimal UnitAmount { get; set; }
        [Display(Name = "Total Amount")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Amount must be greater than 0")]
        public decimal TotalAmount { get; set; }
        [Display(Name = "Commission Percentage")]
        public decimal CommissionPercentage { get; set; }
        [Display(Name = "Commission Amount")]
        public decimal CommissionAmount { get; set; }
        public string? Status { get; set; }
        public string PaymentMode { get; set; } = "CASH";
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public string? GroupNumber { get; set; }
        [Display(Name = "Toli No")]
        public string? LRNumber { get; set; }
        [Display(Name = "LR Number")]
        public string? VehicleNumber { get; set; }
        [Display(Name = "Vehicle Number")]

        public bool IsLeviApplicable { get; set; }
        public string? Levi { get; set; }
        public string? DocketNumber { get; set; }
        public int? TrollyQuantity { get; set; }
        public decimal? TrollyAmount { get; set; }

        public IEnumerable<SelectListItem>? SubContractorList { get; set; }
        public IEnumerable<SelectListItem>? ProductList { get; set; }
    }

}
