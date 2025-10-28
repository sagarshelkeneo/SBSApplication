using Microsoft.AspNetCore.Mvc;
using Client_WebApp.Services.Report;
using Client.Application.Features.PaymentReports.Dtos;
using Client_WebApp.Middleware;

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
                if (!AccessHelper.HasAccess(User, "PAIDREPORT", "View"))
                    return Forbid();

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
                if (!AccessHelper.HasAccess(User, "UNPAIDREPORT", "View"))
                    return Forbid();

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
                if (!AccessHelper.HasAccess(User, "PRODUCTWISEREPORT", "View"))
                    return Forbid();

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
                if (!AccessHelper.HasAccess(User, "SUBCONTRACTORWISEREPORT", "View"))
                    return Forbid();

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
                if (!AccessHelper.HasAccess(User, "COMBINEDREPORT", "View"))
                    return Forbid();

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
