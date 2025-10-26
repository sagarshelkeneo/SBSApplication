using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.User.Dtos
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public int RoleMasterId { get; set; }
        public int CompanyID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int UpdatedBy { get; set; }
    }

}
