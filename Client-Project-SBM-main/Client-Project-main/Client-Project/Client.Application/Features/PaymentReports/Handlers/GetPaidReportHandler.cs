using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.PaymentReports.Dtos;
using Client.Application.Features.PaymentReports.Queries;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.PaymentReports.Handlers
{

    public class GetPaidReportHandler : IRequestHandler<GetPaidReportQuery, List<PaidReportDto>>
    {
        private readonly IReportRepository _repo;
        public GetPaidReportHandler(IReportRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<PaidReportDto>> Handle(GetPaidReportQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetPaidReportAsync(request.SubcontractorName,request.CompanyId,request.BankName,request.FromDate,request.ToDate);
        }
    }

}
