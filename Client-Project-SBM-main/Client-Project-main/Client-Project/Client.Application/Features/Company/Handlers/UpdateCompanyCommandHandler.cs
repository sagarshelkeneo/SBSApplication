using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Client.Application.Features.Company.Commands;
using Client.Application.Features.Product.Dtos;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Company.Handlers
{

    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, List<CompanyDto>>
    {
        private readonly ICompanyRepository _repo;

        public UpdateCompanyCommandHandler(ICompanyRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CompanyDto>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            return await _repo.UpdateCompanyAsync(request.Company);
        }
    }

}
