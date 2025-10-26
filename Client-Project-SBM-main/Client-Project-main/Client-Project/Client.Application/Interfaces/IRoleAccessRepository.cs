using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.RoleAccessControl.Dtos;

namespace Client.Application.Interfaces
{
    public interface IRoleAccessRepository
    {
        Task<string> InsertRoleAccessAsync(RoleAccessDto dto);
        Task<string> UpdateRoleAccessAsync(UpdateRoleAccessDto dto);
        Task<List<UserAccessDto>> GetUserAccessAsync(int? userId, string username);
        Task<List<GetRoleAccessByRoleIdDto>> GetRoleAccessByRoleIdAsync(int id);
    }
}
