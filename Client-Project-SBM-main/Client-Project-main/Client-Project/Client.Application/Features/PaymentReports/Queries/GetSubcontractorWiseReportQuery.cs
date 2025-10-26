using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.PaymentReports.Dtos;
using MediatR;

namespace Client.Application.Features.PaymentReports.Queries
{
    public record GetSubcontractorWiseReportQuery(string? SubcontractorName, int? CompanyId, string? FromDate, string? ToDate) : IRequest<List<SubcontractorWiseReportDto>>;

}
