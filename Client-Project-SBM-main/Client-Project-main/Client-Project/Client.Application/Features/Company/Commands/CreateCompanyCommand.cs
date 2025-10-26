using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Company.Dtos;
using Client.Application.Features.Product.Dtos;
using MediatR;

namespace Client.Application.Features.Company.Commands
{
    public record CreateCompanyCommand(CreateCompanyDto Company) : IRequest<List<CompanyDto>>;
    
}
