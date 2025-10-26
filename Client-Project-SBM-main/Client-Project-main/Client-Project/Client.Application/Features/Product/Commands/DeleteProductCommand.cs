using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Product.Dtos;
using Client.Domain.Models;
using MediatR;

namespace Client.Application.Features.Product.Commands
{
    
    public class DeleteProductCommand : IRequest<List<ProductDto>>
    {
        public int Id { get; set; }
        public int UpdatedBy { get; set; }
        public int CompanyId { get; set; }

        public DeleteProductCommand(int id, int updatedBy, int companyId)
        {
            Id = id;
            UpdatedBy = updatedBy;
            CompanyId = companyId;
        }
    }

}
