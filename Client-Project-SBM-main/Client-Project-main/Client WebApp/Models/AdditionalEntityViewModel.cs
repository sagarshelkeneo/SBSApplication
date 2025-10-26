using Client.Application.Features.AdditionalEntity.Dtos;
using Client.Domain.Models;
using Client_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace Client_WebApp.Models
{
    public class AdditionalEntityViewModel
    {
        public int CompanyId { get; set; }
        public AdditionalEntity AdditionalEntity { get; set; }
        public List<AdditionalEntityDto> AdditionalEntities { get; set; }

        // NEW: Add SubContractors list
        public List<SubContractor> SubContractors { get; set; }

        public AdditionalEntityViewModel()
        {
            AdditionalEntity = new AdditionalEntity();
            AdditionalEntities = new List<AdditionalEntityDto>();
            SubContractors = new List<SubContractor>();
        }
    }

    public class AdditionalEntity
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int SubContractorId { get; set; }

        [Required(ErrorMessage = "*Required")]
        [MaxLength(50)]
        public string Type { get; set; }

        [Required(ErrorMessage = "*Required")]
        [Range(0, double.MaxValue, ErrorMessage = "Invalid Amount")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "*Required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "*Required")]
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
