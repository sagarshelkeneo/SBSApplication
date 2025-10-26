using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Role.Dtos;

namespace Client.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<RoleDto>> GetRolesAsync(int? id);
        Task<List<RoleDto>> CreateRoleAsync(CreateRoleDto dto);
        Task<List<RoleDto>> UpdateRoleAsync(UpdateRoleDto dto);
        Task<List<RoleDto>> DeleteRoleAsync(int id, int updatedBy);


    }
}
