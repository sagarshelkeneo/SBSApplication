using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Product.Dtos;
using Client.Domain.Models;

namespace Client.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductDto>> CreateProductAsync(CreateProductDto dto);
        Task<List<ProductDto>> UpdateProductAsync(UpdateProductDto dto);


        Task<List<ProductDto>> GetProductsAsync(int companyId,int? id, string? search);
        Task<List<ProductDto>> DeleteProductAsync(int id, int updatedBy,int companyId);


    }
}
