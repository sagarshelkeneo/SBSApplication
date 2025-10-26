using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BCrypt.Net;
using Client.Application.Features.User.Dtos;
using Client.Application.Interfaces;
using Client.Domain.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace Client.Persistence.Repositories
{

    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _db;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;


        public UserRepository(IDbConnection db , IConfiguration config, IEmailService emailService, IJwtService jwtService)
        {
            _db = db;
            _config = config;
            _emailService = emailService;
            _jwtService = jwtService;
        }

        //public async Task<List<UserDto>> CreateUserAsync(CreateUserDto userDto)
        //{
        //    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@p_roleMasterId", userDto.RoleMasterId);
        //    parameters.Add("@p_companyId", userDto.CompanyId);
        //    parameters.Add("@p_email", userDto.Email);
        //    parameters.Add("@p_username", userDto.Username);
        //    parameters.Add("@p_hashedPassword", hashedPassword);
        //    parameters.Add("@p_createdBy", userDto.CreatedBy);

        //    var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
        //        "usp_sbs_userMaster_insert",
        //        parameters,
        //        commandType: CommandType.StoredProcedure
        //    );

        //    if (result == null || result.R_Status != "SUCCESS")
        //        throw new Exception($"Insert Failed: {result?.R_ErrorMessage ?? "Unknown error"}");

        //    int insertedId = result.R_InsertedID; 
        //    int companyId = userDto.CompanyId;

        //    return (await GetUsersAsync(null, null, companyId));
        //}
        public async Task<List<UserDto>> CreateUserAsync(CreateUserDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_roleMasterId", dto.RoleMasterId);
            parameters.Add("@p_companyID", dto.CompanyID);
            parameters.Add("@p_username", dto.Username);
            parameters.Add("@p_email", dto.Email);
            parameters.Add("@p_Password", dto.Password);
            parameters.Add("@p_createdBy", dto.CreatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_userMaster_insert",
                parameters,
                commandType: CommandType.StoredProcedure);

            if (result == null || result.R_Status != "SUCCESS")
                throw new Exception($"Insert failed: {result?.R_ErrorMessage ?? "Unknown error"}");

            return await GetUsersAsync(null, null, dto.CompanyID);
        }

        public async Task<LoginResponseDto> ValidateUserAsync(LoginDto dto)
        {
            var result = await _db.QueryFirstOrDefaultAsync<LoginResult>(
                "usp_sbs_userMaster_login",
                new { p_username = dto.Username, p_password = dto.Password },
                commandType: CommandType.StoredProcedure
            );

            if (result == null || result.isValid != "True")
            {
                return new LoginResponseDto
                {
                    Token = null,
                    Message = "Invalid username or password"
                };
            }
            var userDetails = await GetUsersAsync(result.user_Id, null, result.company_ID);

            var user = new UserDto
            {
                Id = result.user_Id,
                Username = dto.Username,
                CompanyID = result.company_ID,
                RoleMasterId = userDetails.First().RoleMasterId,
                RoleName = userDetails.First().RoleName,
                isActive = userDetails.First().isActive,
                Email = userDetails.First().Email
            };

            var token = _jwtService.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                Message = "Login successful"
            };
        }

        //public async Task<string> LoginAsync(string username, string password)
        //{
        //    var parameters = new DynamicParameters();
        //    parameters.Add("@p_search", username);

        //    var user = (await _db.QueryFirstOrDefaultAsync<UserLoginDto>(
        //        "usp_sbs_userMaster_get",

        //        parameters,
        //        commandType: CommandType.StoredProcedure
        //    ));

        //    if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        //        throw new UnauthorizedAccessException("Invalid username or password.");

        //    return GenerateJwtToken(user);
        //}

        private string GenerateJwtToken(UserLoginDto user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("user", user.Username),
            new Claim("userId",user.Id.ToString()),
            new Claim("role",user.RoleName),
            new Claim(ClaimTypes.Role,user.RoleName),
            new Claim("companyId",user.CompanyId.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //public async Task<List<UserDto>> UpdateUserAsync(UpdateUserDto userDto)
        //{
        //    var parameters = new DynamicParameters();
        //    parameters.Add("@p_id", userDto.Id);
        //    parameters.Add("@p_roleMasterId", userDto.RoleMasterId);
        //    parameters.Add("@p_companyId", userDto.CompanyId);
        //    parameters.Add("@p_username", userDto.Username);
        //    parameters.Add("@p_updatedBy", userDto.UpdatedBy);
        //    //var verifiedUsers = await GetUsersAsync(userDto.Id, null, userDto.CompanyId);
        //    //var verifiedUser = verifiedUsers.FirstOrDefault();

        //    if(!userDto.NewPassword.IsNullOrEmpty() && !userDto.CurrentPassword.IsNullOrEmpty())
        //    {
        //        if (userDto == null || !BCrypt.Net.BCrypt.Verify(userDto.CurrentPassword, userDto.Password))
        //            throw new UnauthorizedAccessException("Password does not match.");
        //        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.NewPassword);
        //        parameters.Add("@p_password", hashedPassword);
        //    }
        //    else if (userDto.NewPassword.IsNullOrEmpty() && userDto.CurrentPassword.IsNullOrEmpty())
        //    {
        //        parameters.Add("@p_password", userDto.Password);
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Both NewPassword and CurrentPassword must be provided or neither.");
        //    }

        //    var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
        //            "usp_sbs_userMaster_update",
        //            parameters,
        //            commandType: CommandType.StoredProcedure
        //        );

        //    if (result == null || result.R_Status != "SUCCESS")
        //        throw new Exception($"Update failed: {result?.R_ErrorMessage ?? "Unknown error"}");

        //    int updatedId = result.R_UpdatedID;

        //    return await GetUsersAsync(null, null, userDto.CompanyId);
        //}

        public async Task<List<UserDto>> UpdateUserAsync(UpdateUserDto userDto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_id", userDto.Id);
            parameters.Add("@p_roleMasterId", userDto.RoleMasterId);
            parameters.Add("@p_companyID", userDto.CompanyID);
            parameters.Add("@p_username", userDto.Username);
            parameters.Add("@p_email", userDto.Email);
            parameters.Add("@p_updatedBy", userDto.UpdatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_userMaster_update", parameters, commandType: CommandType.StoredProcedure);

            if (result?.R_Status != "SUCCESS")
                throw new Exception(result?.R_ErrorMessage ?? "Update failed");

            return await GetUsersAsync(null, null, userDto.CompanyID);
        }

        public async Task<List<UserDto>> GetUsersAsync(int? id, string? search, int companyId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_id", id);
            parameters.Add("@p_search", search);
            parameters.Add("@p_companyID", companyId);

            var result = await _db.QueryAsync<UserDto>(
                "usp_sbs_userMaster_get",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
        public async Task<List<UserDto>> DeleteUserAsync(int Id, int updatedBy,int companyId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_id", Id);
            parameters.Add("@p_updatedBy", updatedBy);

            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_userMaster_delete",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            if (result == null || result.R_Status != "SUCCESS")
                throw new Exception($"Update failed: {result?.R_ErrorMessage ?? "Unknown error"}");

            return await GetUsersAsync(null, null, companyId);

        }
        public async Task<List<UserDto>> ToggleIsActiveAsync(ToggleUserActiveDto dto, int companyId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_id", dto.Id);
            parameters.Add("@p_isActive", dto.IsActive);

            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_userMaster_isActive",
                parameters,
                commandType: CommandType.StoredProcedure);

            if (result?.R_Status != "SUCCESS")
                throw new Exception(result?.R_ErrorMessage ?? "Failed to update active status.");

            return await GetUsersAsync(null, null, companyId);
        }

        public async Task<string> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_username", dto.Username);
            parameters.Add("@p_current_password", dto.CurrentPassword);
            parameters.Add("@p_new_password", dto.NewPassword);

            var result = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "usp_sbs_userMaster_change_password", parameters, commandType: CommandType.StoredProcedure);

            if (result?.R_Status != "SUCCESS")
                throw new Exception(result?.R_ErrorMessage ?? "Change Password failed");

            string emailSubject = "Your account information has been updated";
            var sb = new StringBuilder();
            sb.AppendLine($"<p>Dear {dto.Username},</p>");
            sb.AppendLine("<p>Your account has been successfully updated with the following changes:</p>");
            
            sb.AppendLine("<p>✅ Your password has been changed.</p>");
            
            sb.AppendLine("<p>If you did not initiate this change, please contact support immediately.</p>");
            sb.AppendLine("<br/><p>Thank you ❤️</p>");

            await _emailService.SendEmailAsync(dto.Email, emailSubject, sb.ToString());

            return "Password changed successfully.";
        }

        public async Task<bool> VerifyRecaptchaAsync(string token)
        {
            var secretKey = _config["GoogleCaptcha:SecretKey"];
            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
              new KeyValuePair<string, string>("secret", secretKey),
              new KeyValuePair<string, string>("response", token)
            });

            var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var captchaResponse = JsonSerializer.Deserialize<GoogleCaptchaResponse>(responseString);

            return captchaResponse?.success == true;
        }


    }
}
