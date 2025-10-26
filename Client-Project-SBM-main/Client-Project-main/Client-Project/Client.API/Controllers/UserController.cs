using Client.API.Authorization.Attributes;
using Client.Application.Features.User.Commands;
using Client.Application.Features.User.Dtos;
using Client.Application.Features.User.Queries;
using Client.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]

    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _userRepository;

        public UserController(IMediator mediator , IUserRepository userRepository)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }

        [HttpPost]
        //[ScreenAccess("USER","Create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var result = await _mediator.Send(new CreateUserCommand(dto));
            return Ok(result);
        }

        [HttpPut]
        [ScreenAccess("USER", "Edit")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
        {
            var allUsers = await _mediator.Send(new UpdateUserCommand(dto));
            return Ok(allUsers);
        }
        [HttpDelete("{Id}")]
        [ScreenAccess("USER", "Delete")]
        public async Task<IActionResult> DeleteUser([FromRoute]int Id,[FromQuery] int UpdatedBy, [FromQuery]int CompanyId)
        {
            var result = await _mediator.Send(new DeleteUserCommand(Id,UpdatedBy,CompanyId));

            if (result != null)
                return Ok(result);

            return BadRequest(new { message = "Check SP" });
        }
        [HttpPut("toggle-active")]
        [ScreenAccess("USER", "Edit")]
        public async Task<IActionResult> ToggleActive([FromBody] ToggleUserActiveDto dto, int companyId)
        {
            var result = await _mediator.Send(new ToggleUserActiveCommand { Dto = dto , CompanyId = companyId});
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            //reCAPTCHA validation
            if (!await _userRepository.VerifyRecaptchaAsync(dto.RecaptchaToken))
                return BadRequest("reCAPTCHA verification failed.");

            // Handle login via MediatR
            var result = await _mediator.Send(new LoginCommand { Dto = dto });

            if (string.IsNullOrEmpty(result.Token))
                return Unauthorized(result.Message ?? "Invalid username or password.");

            return Ok(result);
        }


        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        //{
        //    try
        //    {
        //        if (! await _userRepository.VerifyRecaptchaAsync(loginDto.RecaptchaToken))
        //            return BadRequest("reCAPTCHA verification failed.");
        //        var token = await _userRepository.LoginAsync(loginDto.Username, loginDto.Password);
        //        return Ok(new { token });
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        return Unauthorized("Invalid username or password");
        //    }
        //}
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var result = await _mediator.Send(new ChangePasswordCommand { PasswordDto = dto });
            return Ok(new {result = result});
        }



        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int companyId,[FromQuery] int? id, [FromQuery] string? search)
        {
            var result = await _mediator.Send(new GetUsersQuery(companyId,id, search ));
            return Ok(result);
        }
    }

}
