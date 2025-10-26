
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Payment.Dtos;

namespace Client.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<List<PaymentDetailsDto>> CreatePaymentAsync(CreatePaymentDto payment);
        Task<List<PaymentDetailsDto>> UpdatePaymentAsync(UpdatePaymentDto dto);
        Task<List<PaymentDetailsDto>> DeletePaymentAsync(int id, int updatedBy,int companyId);

        Task<List<PaymentDetailsDto>> GetPaymentsAsync(int companyId , int? id = null);

    }
}
