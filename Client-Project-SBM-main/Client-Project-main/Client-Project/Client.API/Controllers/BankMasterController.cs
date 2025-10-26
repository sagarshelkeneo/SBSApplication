using Client.API.Authorization.Attributes;
using Client.Application.Features.Bank.Commands;
using Client.Application.Features.Bank.Dtos;
using Client.Application.Features.Bank.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[ScreenAccess("BANK", "View")]

    public class BankMasterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BankMasterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ScreenAccess("BANK", "Create")]
        public async Task<ActionResult<List<BankMasterDto>>> Create([FromBody] CreateBankMasterDto dto)
        {
            var result = await _mediator.Send(new CreateBankMasterCommand(dto));
            return Ok(result);
        }

        [HttpPut]
        [ScreenAccess("BANK", "Edit")]
        public async Task<ActionResult<List<BankMasterDto>>> Update([FromBody] UpdateBankMasterDto dto)
        {
            var result = await _mediator.Send(new UpdateBankMasterCommand(dto));
            return Ok(result);
        }
        [HttpDelete]
        [ScreenAccess("BANK", "Delete")]
        public async Task<ActionResult<List<BankMasterDto>>> Delete(int id, [FromQuery] int updatedBy)
        {
            var dto = new DeleteBankMasterDto { Id = id, UpdatedBy = updatedBy };
            var result = await _mediator.Send(new DeleteBankMasterCommand(dto));
            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? id)
        {
            var result = await _mediator.Send(new GetBankMasterQuery(id));
            return Ok(result);
        }
    }
}
