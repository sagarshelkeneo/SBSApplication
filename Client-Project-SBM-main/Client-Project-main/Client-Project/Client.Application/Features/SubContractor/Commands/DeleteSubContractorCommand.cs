using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.SubContractor.Dtos;
using MediatR;

namespace Client.Application.Features.SubContractor.Commands
{
    public record DeleteSubContractorCommand(int Id,int updatedBy, int companyId) : IRequest<List<SubContractorDto>>;

}
