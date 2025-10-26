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
    public class GetSubcontractorWiseReportHandler : IRequestHandler<GetSubcontractorWiseReportQuery, List<SubcontractorWiseReportDto>>
    {
        private readonly IReportRepository _repo;
        public GetSubcontractorWiseReportHandler(IReportRepository repo)
        {
            _repo = repo;
        }
        public async Task<List<SubcontractorWiseReportDto>> Handle(GetSubcontractorWiseReportQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetSubcontractorWiseReportAsync(request.SubcontractorName, request.CompanyId,request.FromDate,request.ToDate);
        }
    }
}
