using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.RoleAccessControl.Dtos
{
    public class UpdateRoleAccessDto
    {
        public int Id { get; set; }
        public bool ViewAccess { get; set; }
        public bool CreateAccess { get; set; }
        public bool EditAccess { get; set; }
        public bool DeleteAccess { get; set; }
        public int UpdatedBy { get; set; }
    }
}
