using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.AdditionalEntity.Dtos;
using Client.Application.Features.AdditionalEntity.Queries;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.AdditionalEntity.Handlers
{
    public class GetAdditionalEntitiesHandler : IRequestHandler<GetAdditionalEntitiesQuery, List<AdditionalEntityDto>>
    {
        private readonly IAdditionalEntityRepository _repo;

        public GetAdditionalEntitiesHandler(IAdditionalEntityRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<AdditionalEntityDto>> Handle(GetAdditionalEntitiesQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetAsync(request.CompanyId, request.Id, request.SubContractorId);
        }
    }
}
