using Client.Application.Features.Product.Dtos;

namespace Client_WebApp.Models.Master
{
    public class ProductViewModel
    {
        public int CompanyId { get; set; }
        public ProductDto Product { get; set; }
        public List<ProductDto> Products { get; set; }

        public ProductViewModel()
        {
            Product = new ProductDto();
            Products = new List<ProductDto>();
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
