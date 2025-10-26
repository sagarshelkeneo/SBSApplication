using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Product.Dtos;
using MediatR;

namespace Client.Application.Features.Product.Commands
{ 
    public record UpdateProductCommand(UpdateProductDto Product) : IRequest<List<ProductDto>>;

}
