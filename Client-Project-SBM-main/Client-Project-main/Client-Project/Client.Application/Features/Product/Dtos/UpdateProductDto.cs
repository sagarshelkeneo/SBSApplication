using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.Product.Dtos
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int CompanyId { get; set; }
        public int UpdatedBy { get; set; }
    }

}
