using Client.Application.Features.PaymentReports.Dtos;
using Client_WebApp.Middleware;
using Client_WebApp.Services.Report;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Client_WebApp.Controllers.Reports
{
    public class ReportController : BaseController
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService)
        {
            _reportService = reportService;
        }

        #region  isLevhiApplicable = false
        public async Task<IActionResult> ContractorReport(string? fromDate, string? toDate, string? subcontractorName, string? bankName)
        {
            try
            {
                if (!AccessHelper.HasAccess(User, "INVOICE", "View"))
                    return Forbid();

                int? companyId = CurrentCompanyId;

                var reports = await _reportService.GetPaidReportAsync(
                    subcontractorName,
                    companyId,
                    bankName,
                    fromDate,
                    toDate,
                    false
                );

                return View(reports);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to load Contractor Report: {ex.Message}";
                return View(new List<PaidReportDto>());
            }
        }

        public async Task<IActionResult> ProfitLossReport(string? fromDate, string? toDate, string? subcontractorName)
        {
            try
            {
                if (!AccessHelper.HasAccess(User, "INVOICE", "View"))
                    return Forbid();

                int? companyId = CurrentCompanyId;

                var reports = await _reportService.GetUnpaidReportAsync(
                    subcontractorName,
                    companyId,
                    fromDate,
                    toDate,
                    false
                );

                return View(reports);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to load Profit & Loss Report: {ex.Message}";
                return View(new List<UnpaidReportDto>());
            }
        }

        public async Task<IActionResult> ProductWise(string? productName, string? subcontractorName, string? fromDate, string? toDate)
        {
            try
            {
                if (!AccessHelper.HasAccess(User, "INVOICE", "View"))
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
                if (!AccessHelper.HasAccess(User, "INVOICE", "View"))
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

        public async Task<IActionResult> CombinedSubContractorEntity(string? subcontractorName, string? fromDate, string? toDate, bool isTrollyApplicable)
        {
            try
            {
                if (!AccessHelper.HasAccess(User, "INVOICE", "View"))
                    return Forbid();

                var companyId = CurrentCompanyId;
                var data = await _reportService.GetCombinedSubcontractorReportAsync(subcontractorName, companyId, fromDate, toDate, true);

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load report. " + ex.Message;
                return View(new List<CombinedSubcontractorReportDto>());
            }
        }

        public async Task<IActionResult> CombinedSubContractorIndoListEntity(string? subcontractorName, string? fromDate, string? toDate, bool isTrollyApplicable)
        {
            try
            {
                if (!AccessHelper.HasAccess(User, "INVOICE", "View"))
                    return Forbid();

                var companyId = CurrentCompanyId;
                var data = await _reportService.GetCombinedSubcontractorReportAsync(subcontractorName, companyId, fromDate, toDate, false);

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load report. " + ex.Message;
                return View(new List<CombinedSubcontractorReportDto>());
            }
        }

        #endregion

        #region isLevhiApplicable = true
        public async Task<IActionResult> ContractorLegrandReport(string? fromDate, string? toDate, string? subcontractorName, string? bankName)
        {
            try
            {
                if (!AccessHelper.HasAccess(User, "INVOICE", "View"))
                    return Forbid();

                int? companyId = CurrentCompanyId;

                var reports = await _reportService.GetPaidReportAsync(
                    subcontractorName,
                    companyId,
                    bankName,
                    fromDate,
                    toDate,
                    true
                );

                return View(reports);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to load Contractor Report: {ex.Message}";
                return View(new List<PaidReportDto>());
            }
        }

        public async Task<IActionResult> ProfitLossLegrandReport(string? fromDate, string? toDate, string? subcontractorName)
        {
            try
            {
                if (!AccessHelper.HasAccess(User, "INVOICE", "View"))
                    return Forbid();

                int? companyId = CurrentCompanyId;

                var reports = await _reportService.GetUnpaidReportAsync(
                    subcontractorName,
                    companyId,
                    fromDate,
                    toDate,
                    true
                );

                return View(reports);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to load Profit & Loss Report: {ex.Message}";
                return View(new List<UnpaidReportDto>());
            }
        }
        #endregion
    }
}
