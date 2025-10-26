using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Product.Commands;
using Client.Application.Features.Product.Dtos;
using Client.Application.Interfaces;
using Client.Domain.Models;
using MediatR;

namespace Client.Application.Features.Product.Handlers
{
    
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, List<ProductDto>>
    {
        private readonly IProductRepository _repo;

        public DeleteProductCommandHandler(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<ProductDto>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            return await _repo.DeleteProductAsync(request.Id, request.UpdatedBy,request.CompanyId);
        }
    }

}
