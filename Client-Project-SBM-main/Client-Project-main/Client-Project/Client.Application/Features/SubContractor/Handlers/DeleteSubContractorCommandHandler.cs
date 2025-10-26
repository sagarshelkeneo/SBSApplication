using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.SubContractor.Commands;
using Client.Application.Features.SubContractor.Dtos;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.SubContractor.Handlers
{
    public class DeleteSubContractorCommandHandler : IRequestHandler<DeleteSubContractorCommand, List<SubContractorDto>>
    {
        private readonly ISubContractorRepository _repository;

        public DeleteSubContractorCommandHandler(ISubContractorRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SubContractorDto>> Handle(DeleteSubContractorCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteSubContractorAsync(request.Id,request.updatedBy,request.companyId);
        }
    }

}
