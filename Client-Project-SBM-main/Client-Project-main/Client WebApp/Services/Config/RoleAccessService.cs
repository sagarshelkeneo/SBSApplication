using Client.Application.Features.Role.Dtos;
using Client.Application.Features.RoleAccessControl.Commands;
using Client.Application.Features.RoleAccessControl.Dtos;
using Client.Application.Interfaces;

namespace Client_WebApp.Services.Config
{
    public class RoleAccessService
    {

        private readonly IRoleAccessRepository _repository;

        public RoleAccessService(IRoleAccessRepository repository)
        {
            _repository = repository;
        }

        public Task<List<GetRoleAccessByRoleIdDto>> GetRoleAccessByRoleIdAsync(int id)
        {
            return _repository.GetRoleAccessByRoleIdAsync(id);
        }

        public Task<string> CreateRoleAccessAsync(RoleAccessDto dto)
        {
            return _repository.InsertRoleAccessAsync(dto);
        }

        public Task<string> UpdateRoleAccessAsync(UpdateRoleAccessDto dto)
        {
            return _repository.UpdateRoleAccessAsync(dto);
        }

        public Task<List<UserAccessDto>> GetRoleAccessAsync(int? id, string username)
        {
            return _repository.GetUserAccessAsync(id, username);
        }


    }
}
