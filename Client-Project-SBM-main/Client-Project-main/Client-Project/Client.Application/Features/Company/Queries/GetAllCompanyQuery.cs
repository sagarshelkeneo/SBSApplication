using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Product.Dtos;
using MediatR;

namespace Client.Application.Features.Product.Queries
{
    public record GetAllCompanyQuery(int? companyId = null, string? search = null) : IRequest<List<CompanyDto>>;

    

}
