using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.AdditionalEntity.Dtos
{ 
    public class AdditionalEntityDto
    {
        public int R_id { get; set; }
        public string R_type { get; set; }
        public decimal R_amount { get; set; }
        public int R_quantity { get; set; }
        public DateTime R_date { get; set; }
        public int R_companyId { get; set; }
        public int R_subContractorId { get; set; }
        public string R_subContractorName { get; set; }
    }

}
