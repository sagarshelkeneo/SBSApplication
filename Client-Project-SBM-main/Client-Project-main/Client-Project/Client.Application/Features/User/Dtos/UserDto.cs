using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.User.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public int RoleMasterId { get; set; }
        public string RoleName { get; set; }
        public int isActive { get; set; }
        public int CompanyID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }


}
