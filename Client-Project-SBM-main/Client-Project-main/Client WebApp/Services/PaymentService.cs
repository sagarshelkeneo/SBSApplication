using Client.Application.Features.Payment.Dtos;
using Client.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client_WebApp.Services
{
    public class PaymentService
    {
        private readonly IPaymentRepository _repository;

        public PaymentService(IPaymentRepository repository)
        {
            _repository = repository;
        }

        public Task<List<PaymentDetailsDto>> GetPaymentsAsync(int companyId, int? id = null)
        {
            return _repository.GetPaymentsAsync(companyId, id);
        }

        public Task<List<PaymentDetailsDto>> CreatePaymentAsync(CreatePaymentDto dto)
        {
            return _repository.CreatePaymentAsync(dto);
        }

        public Task<List<PaymentDetailsDto>> UpdatePaymentAsync(UpdatePaymentDto dto)
        {
            return _repository.UpdatePaymentAsync(dto);
        }

        public Task<List<PaymentDetailsDto>> DeletePaymentAsync(int id, int updatedBy, int companyId)
        {
            return _repository.DeletePaymentAsync(id, updatedBy, companyId);
        }
    }
}
