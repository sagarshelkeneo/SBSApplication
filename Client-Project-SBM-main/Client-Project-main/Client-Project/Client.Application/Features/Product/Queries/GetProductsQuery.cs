using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Product.Dtos;
using MediatR;

namespace Client.Application.Features.Product.Queries
{

    public class GetProductsQuery : IRequest<List<ProductDto>>
    {
        public int CompanyId { get; set; }
        public int? Id { get; }
        public string Search { get; }

        public GetProductsQuery(int companyId,int? id = null, string search = null)
        {
            CompanyId = companyId;
            Id = id;
            Search = search;
            
        }
    }

}
