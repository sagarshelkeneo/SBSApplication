using Microsoft.AspNetCore.Mvc.Rendering;
using System;
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
        public string InvoiceNo { get; set; }
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
    }

    public class InvoiceViewModel
    {
        public int Id { get; set; }

        public string? InvoiceNo { get; set; }

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
        public string InvoiceNo { get; set; }
        public int CompanyId { get; set; }
        [Display(Name = "Sub Contractor")]
        public int SubcontractorId { get; set; }
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Amount")]
        public decimal UnitAmount { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        [Display(Name = "Commission Percentage")]
        public decimal CommissionPercentage { get; set; }
        [Display(Name = "Commission Amount")]
        public decimal CommissionAmount { get; set; }
        public string? Status { get; set; }
        public string PaymentMode { get; set; } = "CASH";
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public IEnumerable<SelectListItem>? SubContractorList { get; set; }
        public IEnumerable<SelectListItem>? ProductList { get; set; }
    }

}
