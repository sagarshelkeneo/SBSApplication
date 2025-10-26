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
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, List<CompanyDto>>
    {
        public ICompanyRepository _repo;

        public CreateCompanyCommandHandler(ICompanyRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var result = await _repo.CreateCompanyAsync(request.Company);

            return result;
        }
    }
}
