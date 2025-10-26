using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Domain.Models
{
    public class DeleteProductResultDto
    {
        public string R_Status { get; set; }
        public int? R_DeletedID { get; set; }
        public int? R_ErrorNumber { get; set; }
        public string R_ErrorMessage { get; set; }
    }

}
