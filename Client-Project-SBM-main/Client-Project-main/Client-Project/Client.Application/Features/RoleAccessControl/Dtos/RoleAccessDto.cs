using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.RoleAccessControl.Dtos
{
    public class RoleAccessDto
    {
        public int RoleId { get; set; }
        public string ScreenName { get; set; }
        public string ScreenCode { get; set; }
        public bool ViewAccess { get; set; }
        public bool CreateAccess { get; set; }
        public bool EditAccess { get; set; }
        public bool DeleteAccess { get; set; }
        public int CreatedBy { get; set; }
    }
}
