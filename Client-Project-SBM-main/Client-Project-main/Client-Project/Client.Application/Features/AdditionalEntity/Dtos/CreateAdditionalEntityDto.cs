using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.AdditionalEntity.Dtos
{
    public class CreateAdditionalEntityDto
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public int CompanyId { get; set; }
        public int SubContractorId { get; set; }
        public int CreatedBy { get; set; }
    }

}
