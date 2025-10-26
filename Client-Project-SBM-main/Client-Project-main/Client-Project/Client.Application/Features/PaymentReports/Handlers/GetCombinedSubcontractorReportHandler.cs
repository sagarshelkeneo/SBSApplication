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

    public class GetCombinedSubcontractorReportHandler : IRequestHandler<GetCombinedSubcontractorReportQuery, List<CombinedSubcontractorReportDto>>
    {
        private readonly IReportRepository _repository;

        public GetCombinedSubcontractorReportHandler(IReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CombinedSubcontractorReportDto>> Handle(GetCombinedSubcontractorReportQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetCombinedSubcontractorReportAsync();
        }
    }

}
