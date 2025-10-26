using Client.API.Authorization.Attributes;
using Client.Application.Features.AdditionalEntity.Queries;
using Client.Application.Features.RoleAccessControl.Dtos;
using Client.Application.Features.RoleAccessControl.Queries;
using Client.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[ScreenAccess("ROLE_ACCESS", "View")]
    public class RoleAccessController : ControllerBase
    {
        private readonly IRoleAccessRepository _accessService;
        private readonly IMediator _mediator;

        public RoleAccessController(IRoleAccessRepository accessService, IMediator mediator)
        {
            _accessService = accessService;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> InsertRoleAccess([FromBody] RoleAccessDto dto)
        {
            var result = await _accessService.InsertRoleAccessAsync(dto);
            return Ok(new { status = result });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRoleAccess([FromBody] UpdateRoleAccessDto dto)
        {
            var result = await _accessService.UpdateRoleAccessAsync(dto);
            return Ok(new { status = result });
        }


        [HttpGet]
        public async Task<IActionResult> GetUserAccess([FromQuery] int? userId, [FromQuery] string username)
        {
            var accessList = await _accessService.GetUserAccessAsync(userId, username);
            return Ok(accessList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleAccessByRoleId([FromRoute] int id)
        {
            var accessList = await _mediator.Send(new GetRoleAccessByRoleIdQuery(id));
            return Ok(accessList);
        }
    }
}
