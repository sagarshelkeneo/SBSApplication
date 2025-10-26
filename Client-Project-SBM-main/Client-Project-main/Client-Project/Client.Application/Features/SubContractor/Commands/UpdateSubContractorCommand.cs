using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.SubContractor.Dtos;
using MediatR;

namespace Client.Application.Features.SubContractor.Commands
{
    public record UpdateSubContractorCommand(UpdateSubContractorDto SubContractor) : IRequest<List<SubContractorDto>>;

}
