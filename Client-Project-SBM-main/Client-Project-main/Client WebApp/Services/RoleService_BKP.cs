using Client.Application.Features.Role.Dtos;
using Client.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client_WebApp.Services
{
    public class RoleService_BKP
    {
        private readonly IRoleRepository _repository;

        public RoleService_BKP(IRoleRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get all roles or a specific role by ID.
        /// </summary>
        public Task<List<RoleDto>> GetAllRolesAsync(int? id = null)
        {
            return _repository.GetRolesAsync(id);
        }

        /// <summary>
        /// Create a new role.
        /// </summary>
        public async Task<List<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return await _repository.CreateRoleAsync(dto);
        }

        /// <summary>
        /// Update an existing role.
        /// </summary>
        public async Task<List<RoleDto>> UpdateRoleAsync(UpdateRoleDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return await _repository.UpdateRoleAsync(dto);
        }

        /// <summary>
        /// Delete a role by ID.
        /// </summary>
        public async Task<List<RoleDto>> DeleteRoleAsync(int id, int updatedBy)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Role ID.", nameof(id));

            return await _repository.DeleteRoleAsync(id, updatedBy);
        }
    }
}
