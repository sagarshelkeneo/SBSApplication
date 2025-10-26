using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.Company.Dtos
{
    public class SendCompanyEmailDto
    {
        public int CompanyId { get; set; }
        public string ToEmail { get; set; }
        public string? CcEmail { get; set; }
    }
}
