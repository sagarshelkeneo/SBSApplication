using Client.Application.Features.Bank.Dtos;
using Client.Application.Features.SalaryDetails.Dtos;
using Client.Domain.Models;
using Client_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace Client_WebApp.Models
{
    public class SalaryDetailsViewModel
    {
        public int CompanyId { get; set; }
        public SalaryDetails SalaryDetails { get; set; }
        public List<SalaryDetailsDto> SalaryDetailList { get; set; }

        // NEW: Add Banks list
        public List<Bank> Banks { get; set; }

        public SalaryDetailsViewModel()
        {
            SalaryDetails = new SalaryDetails();
            SalaryDetailList = new List<SalaryDetailsDto>();
            Banks = new List<Bank>();
        }
    }

    public class SalaryDetails
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int BankId { get; set; }

        [Required(ErrorMessage = "*Required")]
        [MaxLength(50)]
        public string ToliNo { get; set; }

        [Required(ErrorMessage = "*Required")]
        [Range(0, double.MaxValue, ErrorMessage = "Invalid Amount")]
        public decimal Amount { get; set; }

        //[Required(ErrorMessage = "*Required")]
        //[Range(0, int.MaxValue, ErrorMessage = "Invalid Quantity")]
        //public int Quantity { get; set; }

        [Required(ErrorMessage = "*Required")]
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
