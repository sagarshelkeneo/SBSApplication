using Client.API.Authorization.Attributes;
using Client.Application.Features.SalaryDetails.Commands;
using Client.Application.Features.SalaryDetails.Dtos;
using Client.Application.Features.SalaryDetails.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    //[ScreenAccess("SalaryDetails", "View")]

    public class SalaryDetailsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SalaryDetailsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ScreenAccess("SalaryDetails", "Create")]

        public async Task<IActionResult> Create([FromBody] CreateSalaryDetailsDto dto)
        {
            var result = await _mediator.Send(new CreateSalaryDetailsCommand(dto));
            return Ok(result);
        }

        [HttpPut]
        [ScreenAccess("SalaryDetails", "Edit")]

        public async Task<IActionResult> Update([FromBody] UpdateSalaryDetailsDto dto)
        {
            var result = await _mediator.Send(new UpdateSalaryDetailsCommand(dto));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ScreenAccess("SalaryDetails", "Delete")]

        public async Task<IActionResult> Delete(int id, [FromQuery] int updatedBy, [FromQuery] int companyId)
        {
            var result = await _mediator.Send(new DeleteSalaryDetailsCommand(id, updatedBy, companyId));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int companyId, [FromQuery] int? id = null, [FromQuery] int? subContractorId = null)
        {
            var result = await _mediator.Send(new GetSalaryDetailsQuery(companyId, id, subContractorId));
            return Ok(result);
        }
    }

}
