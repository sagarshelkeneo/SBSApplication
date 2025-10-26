using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.PaymentReports.Dtos;

namespace Client.Application.Interfaces
{
    public interface IReportRepository
    {
        Task<List<PaidReportDto>> GetPaidReportAsync(string? subcontractorName, int? companyId, string? bankName, string fromDate, string toDate);
        Task<List<UnpaidReportDto>> GetUnpaidReportAsync(string? subcontractorName, int? companyId, string fromDate, string toDate);
        Task<List<ProductWiseReportDto>> GetProductWiseReportAsync(string? productName, string? subcontractorName, int? companyId, string fromDate, string toDate);
        Task<List<SubcontractorWiseReportDto>> GetSubcontractorWiseReportAsync(string? subcontractorName, int? companyId, string fromDate, string toDate);
        Task<List<CombinedSubcontractorReportDto>> GetCombinedSubcontractorReportAsync();

    }

}
