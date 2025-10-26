using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.User.Dtos;

namespace Client.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserDto>> CreateUserAsync(CreateUserDto userDto);
        Task<List<UserDto>> UpdateUserAsync(UpdateUserDto userDto);
        Task<List<UserDto>> DeleteUserAsync(int id, int updatedBy,int companyId);
        //Task<string> LoginAsync(string username, string password);
        Task<bool> VerifyRecaptchaAsync(string token);
        Task<List<UserDto>> GetUsersAsync(int? id, string? search,int companyId);
        Task<string> ChangePasswordAsync(ChangePasswordDto dto);
        Task<List<UserDto>> ToggleIsActiveAsync(ToggleUserActiveDto dto, int companyId);
        Task<LoginResponseDto> ValidateUserAsync(LoginDto dto);



    }
}
