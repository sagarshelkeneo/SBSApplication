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

        public Task<List<InvoiceDetailsDto>> GetInvoicesAsync(bool IsLeviApplicable, int companyId, int? id = null)
        {
            return _repository.GetInvoicesAsync(IsLeviApplicable, companyId, id);
        }

        public Task<List<InvoiceDetailsDto>> CreateInvoiceAsync(CreateInvoiceDto dto)
        {
            return _repository.CreateInvoiceAsync(dto);
        }

        public Task<List<InvoiceDetailsDto>> UpdateInvoiceAsync(UpdateInvoiceDto dto)
        {
            return _repository.UpdateInvoiceAsync(dto);
        }

        public Task<List<InvoiceDetailsDto>> DeleteInvoiceAsync(int id, int updatedBy, int companyId, bool isLeviApplicable)
        {
            return _repository.DeleteInvoiceAsync(id, updatedBy, companyId, isLeviApplicable);
        }

        public Task<List<InvoiceDetailsAsPerContractorDto>> GetInvoiceAsPerContractorAsync(
            int? invoiceId, int? subContractorId, DateTime? paymentDate, DateTime? fromDate, DateTime? toDate, bool isLeviApplicable = false)
        {
            return _repository.GetInvoiceAsPerContractorAsync(invoiceId, subContractorId, paymentDate, fromDate, toDate, isLeviApplicable);
        }
    }
}
