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
    public class GetUnpaidReportHandler : IRequestHandler<GetUnpaidReportQuery, List<UnpaidReportDto>>
    {
        private readonly IReportRepository _repo;
        public GetUnpaidReportHandler(IReportRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<UnpaidReportDto>> Handle(GetUnpaidReportQuery request, CancellationToken cancellationToken)
        {
            var result = await _repo.GetUnpaidReportAsync(request.SubcontractorName,request.CompanyId,request.FromDate,request.ToDate);
            return result;
        }
    }
}
