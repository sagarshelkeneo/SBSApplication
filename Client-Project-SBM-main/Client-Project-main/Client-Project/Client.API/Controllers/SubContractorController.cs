using Client.API.Authorization.Attributes;
using Client.Application.Features.SubContractor.Commands;
using Client.Application.Features.SubContractor.Dtos;
using Client.Application.Features.SubContractor.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ScreenAccess("SUBCONTRACTOR", "View")]

    public class SubContractorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubContractorController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        [ScreenAccess("SUBCONTRACTOR", "Create")]

        public async Task<IActionResult> CreateSubContractor([FromBody] CreateSubContractorDto dto)
        {
            var result = await _mediator.Send(new CreateSubContractorCommand(dto));
            return Ok(result);
        }
        [HttpPut]
        [ScreenAccess("SUBCONTRACTOR", "Edit")]

        public async Task<IActionResult> UpdateSubContractor([FromBody] UpdateSubContractorDto dto)
        {
            var result = await _mediator.Send(new UpdateSubContractorCommand(dto));
            return Ok(result);
        }
        [HttpDelete("{id}")]
        [ScreenAccess("SUBCONTRACTOR", "Delete")]
        public async Task<IActionResult> DeleteSubContractor(int id, [FromQuery]int updatedBy, [FromQuery] int companyId)
        {
            var updatedList = await _mediator.Send(new DeleteSubContractorCommand(id,updatedBy,companyId));
            return Ok(updatedList);
        }



        [HttpGet]
        public async Task<IActionResult> GetSubContractors([FromQuery] int? id, [FromQuery] string? search, [FromQuery] int companyId)
        {
            var result = await _mediator.Send(new GetSubContractorQuery(id, search,companyId));
            return Ok(result);
        }
    }

}
