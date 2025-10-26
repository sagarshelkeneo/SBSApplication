using Client.API.Authorization.Attributes;
using Client.Application.Features.AdditionalEntity.Commands;
using Client.Application.Features.AdditionalEntity.Dtos;
using Client.Application.Features.AdditionalEntity.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    //[ScreenAccess("ADDITIONALENTITY", "View")]

    public class AdditionalEntityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdditionalEntityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ScreenAccess("ADDITIONALENTITY", "Create")]

        public async Task<IActionResult> Create([FromBody] CreateAdditionalEntityDto dto)
        {
            var result = await _mediator.Send(new CreateAdditionalEntityCommand(dto));
            return Ok(result);
        }

        [HttpPut]
        [ScreenAccess("ADDITIONALENTITY", "Edit")]

        public async Task<IActionResult> Update([FromBody] UpdateAdditionalEntityDto dto)
        {
            var result = await _mediator.Send(new UpdateAdditionalEntityCommand(dto));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ScreenAccess("ADDITIONALENTITY", "Delete")]

        public async Task<IActionResult> Delete(int id, [FromQuery] int updatedBy, [FromQuery] int companyId)
        {
            var result = await _mediator.Send(new DeleteAdditionalEntityCommand(id, updatedBy, companyId));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int companyId, [FromQuery] int? id = null, [FromQuery] int? subContractorId = null)
        {
            var result = await _mediator.Send(new GetAdditionalEntitiesQuery(companyId, id, subContractorId));
            return Ok(result);
        }
    }

}
