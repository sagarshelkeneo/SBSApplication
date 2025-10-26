using Client.Application.Features.Product.Dtos;
using Client.Application.Features.SubContractor.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;

namespace Client_WebApp.Models.Config
{


    public class CompanyDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }




    public class CompanyViewModel
    {
        public int CompanyId { get; set; }

        public CompanyDto CompanyDto { get; set; }
        public List<CompanyDto> CompanyDtos { get; set; }

        public CompanyViewModel()
        {
            CompanyDto = new CompanyDto();
            CompanyDtos = new List<CompanyDto>();
        }
    }


}
