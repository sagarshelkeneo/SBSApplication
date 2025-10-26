using Client.Application.Features.Company.Dtos;
using Client.Application.Features.Product.Dtos;
using Client.Application.Features.Role.Dtos;
using Client.Application.Interfaces;

namespace Client_WebApp.Services.Config
{
    public class RoleService
    {

        private readonly IRoleRepository _repository;

        public RoleService(IRoleRepository repository)
        {
            _repository = repository;
        }

        public Task<List<RoleDto>> GetRoleAsync(int? id = null)
        {
            return _repository.GetRolesAsync(id);
        }

        public Task<List<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
        {
            return _repository.CreateRoleAsync(dto);
        }

        public Task<List<RoleDto>> UpdateRoleAsync(UpdateRoleDto dto)
        {
            return _repository.UpdateRoleAsync(dto);
        }

        public Task<List<RoleDto>> DeleteRoleAsync(int id, int updatedBy, int companyId)
        {
            return _repository.DeleteRoleAsync(id, updatedBy);
        }
    }
}
