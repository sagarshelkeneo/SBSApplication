using Client.Application.Features.Payment.Dtos;
using Client_WebApp.Controllers;
using Client_WebApp.Middleware;
using Client_WebApp.Models;
using Client_WebApp.Services;
using Client_WebApp.Services.Master;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace Client_WebApp.MVC.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly PaymentService _service;
        private readonly InvoiceService _invoiceService;
        private readonly SubContractorService _subContractorService;
        private readonly BankService _bankService; 

        public PaymentController(PaymentService service, InvoiceService invoiceService, BankService bankService, SubContractorService subContractorService)
        {
            _service = service;
            _invoiceService = invoiceService;
            _bankService = bankService;
            _subContractorService = subContractorService;
        }

        public async Task<IActionResult> Index(string durationType, DateTime? dayDate, DateTime? fromDate, DateTime? toDate, string bankName)
        {
            if (!AccessHelper.HasAccess(User, "PAYMENT", "View"))
                return Forbid();

            int companyId = CurrentCompanyId;

            // Get all payments
            var payments = await _service.GetPaymentsAsync(companyId, 0, bankName);

            // Apply filters
            if (!string.IsNullOrEmpty(durationType))
            {
                if (durationType == "day" && dayDate.HasValue)
                    payments = payments.Where(p => p.R_paymentDate?.Date == dayDate.Value.Date).ToList();
                else if (durationType == "date" && fromDate.HasValue && toDate.HasValue)
                    payments = payments.Where(p => p.R_fromDate >= fromDate.Value.Date && p.R_toDate <= toDate.Value.Date).ToList();
            }

            //if (!string.IsNullOrWhiteSpace(bankName))
            //    payments = payments.Where(p => (p.R_bankName != null && p.R_bankName.Contains(bankName.Trim(), StringComparison.OrdinalIgnoreCase))
            //                                || (p.R_SubContractorName != null && p.R_SubContractorName.Contains(bankName.Trim(), StringComparison.OrdinalIgnoreCase))
                
            //    ).ToList();

            // Get invoices and banks for dropdowns
            var invoices = await _invoiceService.GetInvoicesAsync(false, companyId, null);
            var subContractors = await _subContractorService.GetAllSubContractorAsync(companyId, null);
            var banks = await _bankService.GetAllBanksAsync();

            var model = new PaymentIndexViewModel
            {
                CompanyId = companyId,
                Payments = payments.Select(p => new PaymentViewModel
                {
                    Id = p.R_id,
                    InvoiceId = p.R_invoiceId,
                    InvoiceNo = p.R_invoiceNo,
                    SubContractorId = p.R_SubContractorID,
                    SubContractorName = p.R_SubContractorName,
                    PaymentDate = p.R_paymentDate,
                    FromDate = p.R_fromDate,
                    ToDate = p.R_toDate,
                    AmountPaid = p.R_amountPaid,
                    BankId = p.R_bankId,
                    BankName = p.R_bankName,
                    DurationType = (p.R_fromDate != null && p.R_toDate != null) ? "duration" : "day",
                    InvoiceType = (p.R_invoiceNo != null) ? "invoice" : "subcontractor"
                }).ToList(),
                AddPaymentViewModel = new AddPaymentViewModel
                {
                    Bankes = banks.Select(b => new SelectListItem { Value = b.R_id.ToString(), Text = b.R_bankNameSelect }).ToList(),
                    Invoices = invoices.Where(i => !string.IsNullOrWhiteSpace(i.R_invoiceNo)).Select(i => new SelectListItem { Value = i.R_invoiceNo?.Trim(), Text = i.R_invoiceNoSelect?.Trim() }).ToList(),
                    SubContractors = subContractors.Select(i => new SelectListItem { Value = i.Id.ToString().Trim(), Text = i.Name?.Trim() }).ToList()
                }
            };

            // Preserve filter values in ViewData
            ViewData["DurationType"] = durationType ?? "day";
            ViewData["DayDate"] = dayDate?.ToString("yyyy-MM-dd");
            ViewData["FromDate"] = fromDate?.ToString("yyyy-MM-dd");
            ViewData["ToDate"] = toDate?.ToString("yyyy-MM-dd");
            ViewData["BankName"] = bankName;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(AddPaymentViewModel model)
        {
            if (model.Id > 0)
            {
                if (!AccessHelper.HasAccess(User, "PAYMENT", "Edit"))
                    return Forbid();
            }
            else
            {
                if (!AccessHelper.HasAccess(User, "PAYMENT", "Create"))
                    return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data. Please check your inputs." });
            }

            // Handle date logic using ModalDurationType
            if (model.ModalDurationType == "day")
            {
                model.FromDate = model.ToDate = null;
            }
            else
            {
                model.PaymentDate = model.FromDate;
            }

            if (model.Id == 0)
            {
                var dto = new CreatePaymentDto
                {
                    CompanyId = model.CompanyId,
                    InvoiceNo = model.InvoiceNo,
                    PaymentDate = model.PaymentDate,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    AmountPaid = model.AmountPaid,
                    BankId = model.BankId,
                    CreatedBy = CurrentUserId,
                    SubContractorID = model.SubContractorId
                };

                await _service.CreatePaymentAsync(dto);
                TempData["SuccessMessage"] = "Bank Payment added successfully!";
            }
            else
            {
                var dto = new UpdatePaymentDto
                {
                    Id = model.Id,
                    CompanyId = model.CompanyId,
                    InvoiceNo = model.InvoiceNo,
                    PaymentDate = model.PaymentDate,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    AmountPaid = model.AmountPaid,
                    BankId = model.BankId,
                    UpdatedBy = CurrentUserId,
                    SubContractorID = model.SubContractorId
                };

                await _service.UpdatePaymentAsync(dto);
                TempData["SuccessMessage"] = "Bank Payment updated successfully!";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetPayment(int id)
        {
            if (!AccessHelper.HasAccess(User, "PAYMENT", "View"))
                return Forbid();

            int companyId = CurrentCompanyId;

            var payment = (await _service.GetPaymentsAsync(companyId, id, string.Empty)).FirstOrDefault();
            if (payment == null)
                return NotFound();

            // Determine mode based on available dates
            string durationType = (payment.R_fromDate != null && payment.R_toDate != null)
                ? "duration"
                : "day";

            string invoiceType = (payment.R_invoiceNo != null) ? "invoice" : "subcontractor";

            // Prepare view/edit friendly model
            var model = new
            {
                Id = payment.R_id,
                InvoiceId = payment.R_invoiceId,
                InvoiceNo = payment.R_invoiceNo,
                SubContractorId = payment.R_SubContractorID,
                SubContractorName = payment.R_SubContractorName,
                CompanyId = companyId,
                AmountPaid = payment.R_amountPaid,
                BankId = payment.R_bankId,
                BankName = payment.R_bankName,
                PaymentStatus = payment.R_paymentStatus,
                DurationType = durationType,
                InvoiceType = invoiceType,
                PaymentDate = payment.R_paymentDate?.ToString("yyyy-MM-dd"),
                FromDate = payment.R_fromDate?.ToString("yyyy-MM-dd"),
                ToDate = payment.R_toDate?.ToString("yyyy-MM-dd")
            };

            return Json(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePayment(int id, int companyId, int updatedBy)
        {
            try
            {
                if (!AccessHelper.HasAccess(User, "PAYMENT", "Delete"))
                    return Forbid();

                await _service.DeletePaymentAsync(id, CurrentUserId, CurrentCompanyId);
                TempData["SuccessMessage"] = "Bank Payment deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete Bank Payment. " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { companyId });
        }

    }
}
