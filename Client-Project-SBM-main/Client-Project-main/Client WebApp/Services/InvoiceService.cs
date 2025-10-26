using Client.Application.Features.Invoice.Dtos;
using Client.Application.Features.Product.Dtos;
using Client.Application.Features.SubContractor.Dtos;
using Client.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client_WebApp.Services
{
    public class InvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly IProductRepository _productRepository;

        public InvoiceService(IInvoiceRepository repository, IProductRepository productRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
        }

        public Task<List<InvoiceDetailsDto>> GetInvoicesAsync(int companyId, int? id = null)
        {
            return _repository.GetInvoicesAsync(companyId, id);
        }

        public Task<List<InvoiceDetailsDto>> CreateInvoiceAsync(CreateInvoiceDto dto)
        {
            return _repository.CreateInvoiceAsync(dto);
        }

        public Task<List<InvoiceDetailsDto>> UpdateInvoiceAsync(UpdateInvoiceDto dto)
        {
            return _repository.UpdateInvoiceAsync(dto);
        }

        public Task<List<InvoiceDetailsDto>> DeleteInvoiceAsync(int id, int updatedBy, int companyId)
        {
            return _repository.DeleteInvoiceAsync(id, updatedBy, companyId);
        }

        //public Task<List<ProductDto>> GetProductsAsync(int companyId, int? id = null)
        //{
        //    string? search = null;
        //    return _productRepository.GetProductsAsync(companyId, id, search);
        //}
    }
}
