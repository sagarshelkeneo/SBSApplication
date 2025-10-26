using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.RoleAccessControl.Dtos
{
    public class UserAccessDto
    {

        public int U_userId { get; set; }
        public string U_username { get; set; }
        public string U_email { get; set; }
        public int U_roleId { get; set; }
        public string U_roleName { get; set; }
        public string A_screenName { get; set; }
        public string A_screenCode { get; set; }
        public bool A_viewAccess { get; set; }
        public bool A_createAccess { get; set; }
        public bool A_editAccess { get; set; }
        public bool A_deleteAccess { get; set; }
    }

}
