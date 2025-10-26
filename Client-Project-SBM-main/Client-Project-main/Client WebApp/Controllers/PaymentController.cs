using Client.Application.Features.Payment.Dtos;
using Client_WebApp.Controllers;
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
        private readonly BankService _bankService; 

        public PaymentController(PaymentService service, InvoiceService invoiceService, BankService bankService)
        {
            _service = service;
            _invoiceService = invoiceService;
            _bankService = bankService;
        }

        public async Task<IActionResult> Index(string durationType, DateTime? dayDate, DateTime? fromDate, DateTime? toDate, string bankName)
        {
            int companyId = CurrentCompanyId;

            // Get all payments
            var payments = await _service.GetPaymentsAsync(companyId);

            // Apply filters
            if (!string.IsNullOrEmpty(durationType))
            {
                if (durationType == "day" && dayDate.HasValue)
                    payments = payments.Where(p => p.R_paymentDate?.Date == dayDate.Value.Date).ToList();
                else if (durationType == "date" && fromDate.HasValue && toDate.HasValue)
                    payments = payments.Where(p => p.R_fromDate >= fromDate.Value.Date && p.R_toDate <= toDate.Value.Date).ToList();
            }

            if (!string.IsNullOrWhiteSpace(bankName))
                payments = payments.Where(p => p.R_bankName != null && p.R_bankName.Contains(bankName.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

            // Get invoices and banks for dropdowns
            var invoices = await _invoiceService.GetInvoicesAsync(companyId, null);
            var banks = await _bankService.GetAllBanksAsync();

            var model = new PaymentIndexViewModel
            {
                CompanyId = companyId,
                Payments = payments.Select(p => new PaymentViewModel
                {
                    Id = p.R_id,
                    InvoiceNo = p.R_invoiceNo,
                    PaymentDate = p.R_paymentDate,
                    FromDate = p.R_fromDate,
                    ToDate = p.R_toDate,
                    AmountPaid = p.R_amountPaid,
                    BankId = p.R_bankId,
                    BankName = p.R_bankName,
                    DurationType = (p.R_fromDate != null && p.R_toDate != null) ? "duration" : "day"
                }).ToList(),
                AddPaymentViewModel = new AddPaymentViewModel
                {
                    Bankes = banks.Select(b => new SelectListItem { Value = b.R_id.ToString(), Text = $"{b.R_bankName} ({b.R_branch})" }).ToList(),
                    Invoices = invoices.Select(i => new SelectListItem { Value = i.R_invoiceNo?.Trim(), Text = i.R_invoiceNo?.Trim() }).ToList()
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
                    CreatedBy = CurrentUserId
                };

                await _service.CreatePaymentAsync(dto);
                TempData["SuccessMessage"] = "Payment added successfully!";
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
                    UpdatedBy = CurrentUserId
                };

                await _service.UpdatePaymentAsync(dto);
                TempData["SuccessMessage"] = "Payment updated successfully!";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetPayment(int id)
        {
            int companyId = CurrentCompanyId;

            var payment = (await _service.GetPaymentsAsync(companyId, id)).FirstOrDefault();
            if (payment == null)
                return NotFound();

            // Determine mode based on available dates
            string durationType = (payment.R_fromDate != null && payment.R_toDate != null)
                ? "duration"
                : "day";

            // Prepare view/edit friendly model
            var model = new
            {
                Id = payment.R_id,
                InvoiceNo = payment.R_invoiceNo,
                CompanyId = companyId,
                AmountPaid = payment.R_amountPaid,
                BankId = payment.R_bankId,
                BankName = payment.R_bankName,
                PaymentStatus = payment.R_paymentStatus,
                DurationType = durationType,
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
            companyId = CurrentCompanyId;
            updatedBy = CurrentUserId;
            try
            {
                await _service.DeletePaymentAsync(id, updatedBy, companyId);
                TempData["SuccessMessage"] = "Payment deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete payment. " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { companyId });
        }

    }
}
