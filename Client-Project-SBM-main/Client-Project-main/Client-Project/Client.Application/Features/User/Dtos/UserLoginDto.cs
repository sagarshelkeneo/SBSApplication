using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.User.Dtos
{

    public class UserLoginDto
    {
        public int Id { get; set; }
        public int RoleMasterId { get; set; }
        public string RoleName { get; set; }
        public int CompanyId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } 
    }

}
