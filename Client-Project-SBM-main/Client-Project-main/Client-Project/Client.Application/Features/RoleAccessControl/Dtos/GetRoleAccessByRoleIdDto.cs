using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Features.RoleAccessControl.Dtos
{
    public class GetRoleAccessByRoleIdDto
    {
        public int A_id { get; set; }
        public int A_roleId {  get; set; }
        public string A_roleName {  get; set; }
        public string A_screenName { get; set; }
        public string A_screenCode { get; set; }
        public bool A_viewAccess { get; set; }
        public bool A_createAccess { get; set; }
        public bool A_editAccess {  get; set; }
        public bool A_deleteAccess { get; set; }
    }
}
