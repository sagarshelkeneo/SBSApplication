using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Client.Application.Features.Product.Commands;
using Client.Application.Features.Product.Dtos;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Product.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand,List<ProductDto>>
    {
        private readonly IProductRepository _repo;

        public CreateProductCommandHandler(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            return await _repo.CreateProductAsync(request.Product);
        }
    }

}
