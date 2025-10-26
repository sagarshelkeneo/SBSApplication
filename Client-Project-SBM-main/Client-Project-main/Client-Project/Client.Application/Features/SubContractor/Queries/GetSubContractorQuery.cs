using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.SubContractor.Dtos;
using MediatR;

namespace Client.Application.Features.SubContractor.Queries
{
    public record GetSubContractorQuery(int? Id, string? Search ,int companyId) : IRequest<List<SubContractorDto>>;

}
