using Client.Application.Features.User.Dtos;
using Client.Application.Interfaces;
using Client.Persistence.Repositories;
using Client_WebApp.Models.Master;

namespace Client_WebApp.Services.Master
{
    public class UserService
    {
        private readonly IUserRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<List<UserDto>> GetAllUsersAsync(int companyId, int? id = null, string? search = null)
        {
            return _repository.GetUsersAsync(id, search, companyId);
        }

        public Task<List<UserDto>> CreateUserAsync(CreateUserDto dto)
        {
            return _repository.CreateUserAsync(dto);
        }

        public Task<List<UserDto>> UpdateUserAsync(UpdateUserDto dto)
        {
            return _repository.UpdateUserAsync(dto);
        }

        public Task<List<UserDto>> DeleteUserAsync(int id, int updatedBy, int companyId)
        {
            return _repository.DeleteUserAsync(id, updatedBy, companyId);
        }

        public Task<List<UserDto>> ToggleActiveAsync(ToggleUserActiveDto dto, int companyId)
        {
            return _repository.ToggleIsActiveAsync(dto, companyId);
        }

        public async Task ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var username = model.Username;

            if (string.IsNullOrEmpty(username))
                throw new Exception("Session expired. Please login again.");

            var dto = new ChangePasswordDto
            {
                Username = username,
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword,
                Email = model.Email
            };

            await _repository.ChangePasswordAsync(dto);
        }
    }
}
