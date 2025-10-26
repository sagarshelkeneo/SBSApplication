using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Company.Dtos;
using Client.Application.Features.Product.Dtos;
using Client.Domain.Models;

namespace Client.Application.Interfaces
{
    public interface ICompanyRepository
    {
        Task<List<CompanyDto>> CreateCompanyAsync(CreateCompanyDto companyDto);
        Task<List<CompanyDto>> UpdateCompanyAsync(UpdateCompanyDto dto);
        Task<List<CompanyDto>> DeleteCompanyAsync(int Id,int updatedBy);


        Task<List<CompanyDto>> GetCompaniesAsync(int? companyId, string? search);

        Task<string> SendCompanyEmailAsync(SendCompanyEmailDto dto);




    }
}
