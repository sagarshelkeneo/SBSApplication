using Client.Application.Features.PaymentReports.Dtos;
using Client.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client_WebApp.Services.Report
{
    public class ReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<List<PaidReportDto>> GetPaidReportAsync(string? subcontractorName, int? companyId, string? bankName, string fromDate, string toDate)
        {
            return await _reportRepository.GetPaidReportAsync(subcontractorName, companyId, bankName, fromDate, toDate);
        }

        public async Task<List<UnpaidReportDto>> GetUnpaidReportAsync(string? subcontractorName, int? companyId, string fromDate, string toDate)
        {
            return await _reportRepository.GetUnpaidReportAsync(subcontractorName, companyId, fromDate, toDate);
        }

        public async Task<List<ProductWiseReportDto>> GetProductWiseReportAsync(
        string? productName, string? subcontractorName, int? companyId, string? fromDate, string? toDate)
        {
            return await _reportRepository.GetProductWiseReportAsync(productName, subcontractorName, companyId, fromDate, toDate);
        }

        public async Task<List<SubcontractorWiseReportDto>> GetSubContractorWiseReportAsync(
            string? subcontractorName, int? companyId, string? fromDate, string? toDate)
        {
            return await _reportRepository.GetSubcontractorWiseReportAsync(subcontractorName, companyId, fromDate, toDate);
        }

        public async Task<List<CombinedSubcontractorReportDto>> GetCombinedSubcontractorReportAsync()
        {
            return await _reportRepository.GetCombinedSubcontractorReportAsync();
        }
    }
}
