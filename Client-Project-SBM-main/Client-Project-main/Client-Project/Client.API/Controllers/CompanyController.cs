using Client.API.Authorization.Attributes;
using Client.Application.Features.Company.Commands;
using Client.Application.Features.Company.Dtos;
using Client.Application.Features.Product.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[ScreenAccess("COMPANY", "View")]
    public class CompanyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CompanyController(IMediator mediator)
        {
            _mediator = mediator;
        }
        //[HttpPost]
        //public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyCommand command)
        //{
        //    var result = await _mediator.Send(command);
        //    return CreatedAtAction(nameof(GetCompanyById), new { Id = result.Id }, result);
        //}
        [HttpPost("create")]
        [ScreenAccess("COMPANY", "Create")]
        public async Task<IActionResult> Create([FromBody] CreateCompanyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new CreateCompanyCommand(dto);
                var result = await _mediator.Send(command);
                return Ok(result); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("update")]
        [ScreenAccess("COMPANY", "Edit")]
        public async Task<IActionResult> Update([FromBody] UpdateCompanyDto dto)
        {
            try
            {
                var result = await _mediator.Send(new UpdateCompanyCommand(dto));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        [ScreenAccess("COMPANY", "Delete")]
        public async Task<IActionResult> Delete(int id,[FromQuery]int updatedBy)
        {
            var result = await _mediator.Send(new DeleteCompanyCommand(id,updatedBy));
             return Ok( result );

        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompanies(int? companyId, [FromQuery] string? search)
        {
            var result = await _mediator.Send(new GetAllCompanyQuery(companyId, search));
            return Ok(result);
        }

        [HttpPost("send-report-email")]
        public async Task<IActionResult> SendEmail([FromBody] SendCompanyEmailDto dto)
        {
            var result = await _mediator.Send(new SendCompanyEmailCommand(dto));
            return Ok(new { status = "Success", message = result });
        }

    }
}
