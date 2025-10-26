using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.AdditionalEntity.Dtos;
using MediatR;

namespace Client.Application.Features.AdditionalEntity.Queries
{
    public record GetAdditionalEntitiesQuery(int CompanyId, int? Id = null, int? SubContractorId = null) : IRequest<List<AdditionalEntityDto>>;

}
