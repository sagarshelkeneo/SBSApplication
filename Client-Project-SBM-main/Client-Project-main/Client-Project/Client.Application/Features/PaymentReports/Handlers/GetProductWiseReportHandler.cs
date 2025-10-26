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
    public class GetProductWiseReportHandler : IRequestHandler<GetProductWiseReportQuery, List<ProductWiseReportDto>>
    {
        private readonly IReportRepository _repo;
        public GetProductWiseReportHandler(IReportRepository repo)
        {
            _repo = repo;
        }
        public async Task<List<ProductWiseReportDto>> Handle(GetProductWiseReportQuery request, CancellationToken cancellationToken)
        {
            var result = await _repo.GetProductWiseReportAsync(request.ProductName,request.SubcontractorName,request.CompanyId,request.FromDate,request.ToDate);
            return result;
        }
    }
}
