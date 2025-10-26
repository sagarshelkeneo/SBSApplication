using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Company.Commands;
using Client.Application.Features.Product.Dtos;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Company.Handlers
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, List<CompanyDto>>
    {
        private readonly ICompanyRepository _repo;

        public DeleteCompanyCommandHandler(ICompanyRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CompanyDto>> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            return await _repo.DeleteCompanyAsync(request.Id,request.updatedBy);
        }
    }

}
