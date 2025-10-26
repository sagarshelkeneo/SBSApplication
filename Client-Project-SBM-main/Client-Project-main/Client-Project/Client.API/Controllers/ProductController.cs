using Client.API.Authorization.Attributes;
using Client.Application.Features.Product.Commands;
using Client.Application.Features.Product.Dtos;
using Client.Application.Features.Product.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[ScreenAccess("PRODUCT", "View")]

    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("create")]
        [ScreenAccess("PRODUCT", "Create")]

        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            
                var result = await _mediator.Send(new CreateProductCommand(dto));
                return Ok(result);
           
        }
        [HttpPut("update")]
        [ScreenAccess("PRODUCT", "Edit")]

        public async Task<IActionResult> Update([FromBody] UpdateProductDto dto)
        {
            
                var result = await _mediator.Send(new UpdateProductCommand(dto));
                return Ok(result);
            
           
        }
        //[HttpGet]
        //public async Task<IActionResult> GetAllProducts()
        //{
        //    var result = await _mediator.Send(new GetAllProductQuery());
        //    return Ok(result);
        //}
        [HttpGet]
        public async Task<IActionResult> Get(int companyId,[FromQuery] int? id, [FromQuery] string? search)
        {
            var products = await _mediator.Send(new GetProductsQuery(companyId,id, search));
            return Ok(products);
        }

        [HttpDelete("{id}")]
        [ScreenAccess("PRODUCT", "Delete")]

        public async Task<IActionResult> Delete(int id, [FromQuery] int updatedBy, [FromQuery] int companyId)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id, updatedBy,companyId));

             return Ok(result);

        }
    }
}
