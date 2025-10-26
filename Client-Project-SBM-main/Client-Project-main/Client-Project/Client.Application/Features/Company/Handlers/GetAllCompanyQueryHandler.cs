using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Client.Application.Features.Product.Dtos;
using Client.Application.Features.Product.Queries;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Product.Handlers
{
    public class GetAllCompanyQueryHandler : IRequestHandler<GetAllCompanyQuery, List<CompanyDto>>
    {
        private readonly ICompanyRepository _companyRepository;
        public GetAllCompanyQueryHandler(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<List<CompanyDto>> Handle(GetAllCompanyQuery request, CancellationToken cancellationToken)
        {
            var result =  await _companyRepository.GetCompaniesAsync(request.companyId,request.search);
            return result;

        }
    }
}
