using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.SubContractor.Dtos;
using Client.Application.Features.SubContractor.Queries;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.SubContractor.Handlers
{
    public class GetSubContractorQueryHandler : IRequestHandler<GetSubContractorQuery, List<SubContractorDto>>
    {
        private readonly ISubContractorRepository _repository;

        public GetSubContractorQueryHandler(ISubContractorRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SubContractorDto>> Handle(GetSubContractorQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetSubContractorsAsync(request.Id, request.Search,request.companyId);
        }
    }

}
