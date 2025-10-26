using Microsoft.AspNetCore.Mvc;
using Client_WebApp.Services.Report;
using Client.Application.Features.PaymentReports.Dtos;

namespace Client_WebApp.Controllers.Reports
{
    public class ReportController : BaseController
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Paid(string? fromDate, string? toDate, string? subcontractorName, string? bankName)
        {
            try
            {
                int? companyId = CurrentCompanyId;

                var reports = await _reportService.GetPaidReportAsync(
                    subcontractorName,
                    companyId,
                    bankName,
                    fromDate,
                    toDate
                );

                return View(reports);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to load Paid Report: {ex.Message}";
                return View(new List<PaidReportDto>());
            }
        }

        public async Task<IActionResult> Unpaid(string? fromDate, string? toDate, string? subcontractorName)
        {
            try
            {
                int? companyId = CurrentCompanyId;

                var reports = await _reportService.GetUnpaidReportAsync(
                    subcontractorName,
                    companyId,
                    fromDate,
                    toDate
                );

                return View(reports);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to load Unpaid Report: {ex.Message}";
                return View(new List<UnpaidReportDto>());
            }
        }

        public async Task<IActionResult> ProductWise(string? productName, string? subcontractorName, string? fromDate, string? toDate)
        {
            try
            {
                var companyId = CurrentCompanyId;
                var data = await _reportService.GetProductWiseReportAsync(productName, subcontractorName, companyId, fromDate, toDate);
                return View(data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load report. " + ex.Message;
                return View(new List<ProductWiseReportDto>());
            }
        }

        public async Task<IActionResult> SubContractorWise(string? subcontractorName, string? fromDate, string? toDate)
        {
            try
            {
                var companyId = CurrentCompanyId;
                var data = await _reportService.GetSubContractorWiseReportAsync(subcontractorName, companyId, fromDate, toDate);
                return View(data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load report. " + ex.Message;
                return View(new List<SubcontractorWiseReportDto>());
            }
        }

        public async Task<IActionResult> CombinedSubContractorEntity()
        {
            try
            {
                var companyId = CurrentCompanyId;
                var data = await _reportService.GetCombinedSubcontractorReportAsync();

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load report. " + ex.Message;
                return View(new List<CombinedSubcontractorReportDto>());
            }
        }
    }
}
