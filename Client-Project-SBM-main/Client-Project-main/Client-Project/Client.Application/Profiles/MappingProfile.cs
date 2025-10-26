using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Client.Application.Features.Company.Dtos;
using Client.Application.Features.Invoice.Dtos;
using Client.Application.Features.Product.Dtos;
using Client.Application.Features.Role.Dtos;
using Client.Domain.Models;

namespace Client.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateCompanyDto, CompanyMaster>().ReverseMap();
            CreateMap<UpdateCompanyDto, CompanyMaster>().ReverseMap();
            CreateMap<CompanyDto, CompanyMaster>().ReverseMap();
           CreateMap<ProductDto,Product>().ReverseMap();
            CreateMap<CreateProductDto, Product>().ReverseMap();
            CreateMap<UpdateProductDto, Product>().ReverseMap();

            CreateMap<InvoiceDetailsDto, InvoiceDetails>().ReverseMap();
            CreateMap<CreateInvoiceDto, InvoiceDetails>().ReverseMap();
            CreateMap<UpdateInvoiceDto, InvoiceDetails>().ReverseMap();
            CreateMap<RoleDto,RoleMaster>().ReverseMap();


        }
    }
}
