using Client.Application.Features.Product.Dtos;
using Client.Application.Interfaces;

namespace Client_WebApp.Services.Master
{
    public class ProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public Task<List<ProductDto>> GetProductsAsync(int companyId, int? id = null, string? search = null)
        {
            return _repository.GetProductsAsync(companyId, id, search);
        }

        public Task<List<ProductDto>> CreateProductAsync(CreateProductDto dto)
        {
            return _repository.CreateProductAsync(dto);
        }

        public Task<List<ProductDto>> UpdateProductAsync(UpdateProductDto dto)
        {
            return _repository.UpdateProductAsync(dto);
        }

        public Task<List<ProductDto>> DeleteProductAsync(int id, int updatedBy, int companyId)
        {
            return _repository.DeleteProductAsync(id, updatedBy, companyId);
        }
    }
}
