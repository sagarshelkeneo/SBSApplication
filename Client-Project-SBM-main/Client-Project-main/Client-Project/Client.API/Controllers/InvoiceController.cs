using Client.API.Authorization.Attributes;
using Client.Application.Features.Invoice.Commands;
using Client.Application.Features.Invoice.Dtos;
using Client.Application.Features.Invoice.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ScreenAccess("INVOICE", "View")]
    public class InvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("create")]
        [ScreenAccess("INVOICE", "Create")]

        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
        {
            var result = await _mediator.Send(new CreateInvoiceCommand(dto));
            return Ok(result);
        }
        [HttpPut("update")]
        [ScreenAccess("INVOICE", "Edit")]

        public async Task<IActionResult> UpdateInvoice([FromBody] UpdateInvoiceDto dto)
        {
            var result = await _mediator.Send(new UpdateInvoiceCommand(dto));
            return Ok(result);
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteInvoice(int id, [FromQuery] int updatedBy)
        //{
        //    var result = await _mediator.Send(new DeleteInvoiceCommand(id, updatedBy));
        //    return Ok(result);
        //}
        [HttpDelete("{id}")]
        [ScreenAccess("INVOICE", "Delete")]

        public async Task<IActionResult> DeleteInvoice(int id, [FromQuery] int updatedBy, [FromQuery] int companyId)
        {
            var result = await _mediator.Send(new DeleteInvoiceCommand(id, updatedBy,companyId));
            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]int companyId,[FromQuery] int? id)
        {
            var result = await _mediator.Send(new GetInvoicesQuery(companyId,id));
            return Ok(result);
        }
    }

}
