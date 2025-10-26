using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Product.Dtos;
using MediatR;

namespace Client.Application.Features.Company.Commands
{
    
    public record DeleteCompanyCommand(int Id,int updatedBy) : IRequest<List<CompanyDto>>;

}
