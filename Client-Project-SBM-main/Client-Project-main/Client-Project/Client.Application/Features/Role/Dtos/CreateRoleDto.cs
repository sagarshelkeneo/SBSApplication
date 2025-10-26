using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.Role.Dtos
{
    public class CreateRoleDto
    {
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
    }

}
