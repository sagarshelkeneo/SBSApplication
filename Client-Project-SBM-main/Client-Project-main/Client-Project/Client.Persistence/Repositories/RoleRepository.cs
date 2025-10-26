using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Role.Dtos;
using Client.Application.Interfaces;
using Dapper;

namespace Client.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbConnection _db;

        public RoleRepository(IDbConnection db)
        {
            _db = db;
        }
        public async Task<List<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_roleName", dto.RoleName);
            parameters.Add("@p_description", dto.Description);
            parameters.Add("@p_createdBy", dto.CreatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<(string status, int? inserted_id)>(
                "usp_sbs_roleMaster_insert",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result.status == "SUCCESS" && result.inserted_id.HasValue)
            {
                return await GetRolesAsync(null);
            }

            throw new Exception("Role creation failed.");
        }
        public async Task<List<RoleDto>> UpdateRoleAsync(UpdateRoleDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_id", dto.Id);
            parameters.Add("@p_roleName", dto.RoleName);
            parameters.Add("@p_description", dto.Description);
            parameters.Add("@p_updatedBy", dto.UpdatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<string>(
                "usp_sbs_roleMaster_update",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result == "SUCCESS")
            {
                return await GetRolesAsync(null);
            }

            throw new Exception("Role update failed: " + result);
        }
        public async Task<List<RoleDto>> DeleteRoleAsync(int id, int updatedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_id", id);
            parameters.Add("@p_updatedBy", updatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<string>(
                "usp_sbs_roleMaster_delete",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result == "SUCCESS")
            {
                return await GetRolesAsync(null);
            }

            throw new Exception(result);
        }
        public async Task<List<RoleDto>> GetRolesAsync(int? id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_id", id);

            var result = await _db.QueryAsync<RoleDto>(
                "usp_sbs_roleMaster_get",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
    }

}
