using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Role.Dtos;
using Client.Application.Features.RoleAccessControl.Dtos;
using Client.Application.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Client.Persistence.Repositories
{
    public class RoleAccessRepository : IRoleAccessRepository
    {
        private readonly IDbConnection _connection;

        public RoleAccessRepository(IDbConnection connection, IConfiguration configuration)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task<string> InsertRoleAccessAsync(RoleAccessDto dto)
        {
            var result = await _connection.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_roleAccessControl_insert",
                new
                {
                    p_roleId = dto.RoleId,
                    p_screenName = dto.ScreenName,
                    p_screenCode = dto.ScreenCode,
                    p_viewAccess = dto.ViewAccess,
                    p_createAccess = dto.CreateAccess,
                    p_editAccess = dto.EditAccess,
                    p_deleteAccess = dto.DeleteAccess,
                    p_createdBy = dto.CreatedBy
                },
                commandType: CommandType.StoredProcedure
            );

            return result?.R_Status ?? "FAIL";
        }

        public async Task<string> UpdateRoleAccessAsync(UpdateRoleAccessDto dto)
        {
            var result = await _connection.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_roleAccessControl_update",
                new
                {
                    p_id = dto.Id,
                    p_viewAccess = dto.ViewAccess,
                    p_createAccess = dto.CreateAccess,
                    p_editAccess = dto.EditAccess,
                    p_deleteAccess = dto.DeleteAccess,
                    p_updatedBy = dto.UpdatedBy
                },
                commandType: CommandType.StoredProcedure
            );

            return result?.R_Status ?? "FAIL";
        }
       
        public async Task<List<UserAccessDto>> GetUserAccessAsync(int? userId, string username)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_userId", userId);
            parameters.Add("@p_username", username);

            var result = await _connection.QueryAsync<UserAccessDto>(
                "usp_sbs_userAccess_get",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<List<GetRoleAccessByRoleIdDto>> GetRoleAccessByRoleIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_roleId", id);

            var result = await _connection.QueryAsync<GetRoleAccessByRoleIdDto>(
                "usp_sbs_roleAccess_get",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
    }
}
