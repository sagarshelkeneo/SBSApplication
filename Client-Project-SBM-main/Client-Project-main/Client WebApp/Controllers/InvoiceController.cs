using Client.Application.Features.Invoice.Dtos;
using Client_WebApp.Controllers;
using Client_WebApp.Models;
using Client_WebApp.Services;
using Client_WebApp.Services.Master;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IdentityModel.Tokens.Jwt;

namespace Client.MVC.Controllers
{
    public class InvoiceController : BaseController
    {
        private readonly InvoiceService _service;
        private readonly SubContractorService _subContractorService;
        private readonly ProductService _productService;

        public InvoiceController(InvoiceService service, SubContractorService subContractorService, ProductService productService)
        {
            _service = service;
            _subContractorService = subContractorService;
            _productService = productService;
        }

        public async Task<IActionResult> Index(string? searchText, DateTime? FromDate, DateTime? ToDate)
        {
            int companyId = CurrentCompanyId;
            // Fetch application DTOs from service
            List<Client.Application.Features.Invoice.Dtos.InvoiceDetailsDto> invoicesFromService =
                await _service.GetInvoicesAsync(companyId);

            // Filter based on FromDate, ToDate, and Subcontractor / Invoice No.
            if (FromDate.HasValue)
            {
                invoicesFromService = invoicesFromService
                    .Where(i => i.R_invoiceDate.Date >= FromDate.Value.Date)
                    .ToList();
            }

            if (ToDate.HasValue)
            {
                invoicesFromService = invoicesFromService
                    .Where(i => i.R_invoiceDate.Date <= ToDate.Value.Date)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.ToLower();
                invoicesFromService = invoicesFromService
                    .Where(i =>
                        (i.R_subcontractorName != null && i.R_subcontractorName.ToLower().Contains(searchText)) ||
                        (i.R_invoiceNo.ToString().ToLower().Contains(searchText))
                    )
                    .ToList();
            }

            // Map to WebApp DTO
            var webInvoices = invoicesFromService.Select(i => new Client_WebApp.Models.InvoiceDetailsDto
            {
                Id = i.R_id,
                InvoiceNo = i.R_invoiceNo,
                CompanyId = i.R_companyId,
                SubContractorId = i.R_subcontractorId,
                SubContractorName = i.R_subcontractorName,
                ProductName = i.R_productName,
                UnitPrice = i.R_unitPrice,
                UnitAmount = i.R_unitAmount,
                InvoiceDate = i.R_invoiceDate,
                Status = i.R_status,
                Quantity = i.R_quantity,
                TotalAmount = i.R_totalAmount,
                CommissionPercentage = i.R_commissionPercentage,
                CommissionAmount = i.R_commissionAmount,
                InvoiceType = i.R_invoiceType
            }).ToList();

            // Pass filter values to ViewData to preserve in form
            ViewData["FromDate"] = FromDate?.ToString("yyyy-MM-dd");
            ViewData["ToDate"] = ToDate?.ToString("yyyy-MM-dd");
            ViewData["searchText"] = searchText;

            var subcontractors = await _subContractorService.GetAllSubContractorAsync(companyId);
            var products = await _productService.GetProductsAsync(companyId);
            var subcontractorList = subcontractors.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();

            var productList = products.Select(s => new SelectListItem
            {
                Value = s.R_id.ToString(),
                Text = s.R_description,
            }).ToList();

            ViewBag.ProductPrices = products.ToDictionary(p => p.R_id, p => p.R_unitPrice);

            var model = new InvoiceIndexViewModel
            {
                NewInvoice = new InvoiceViewModel
                {
                    CompanyId = companyId,
                },
                AddInvoice = new AddInvoiceViewModel
                {
                    InvoiceDate = DateTime.Now,
                    SubContractorList = subcontractorList,
                    ProductList = productList
                },
                Invoices = webInvoices
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(AddInvoiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Id > 0)
            {
                // Update
                var updateDto = new UpdateInvoiceDto
                {
                    Id = model.Id,
                    CompanyId = CurrentCompanyId,
                    SubcontractorId = model.SubcontractorId,
                    ProductId = model.ProductId,
                    InvoiceNo = model.InvoiceNo,
                    InvoiceDate = model.InvoiceDate,
                    UnitAmount = model.UnitAmount,
                    Quantity = model.Quantity,
                    TotalAmount = model.TotalAmount,
                    CommissionPercentage = model.CommissionPercentage,
                    CommissionAmount = model.CommissionAmount,
                    PaymentMode = model.PaymentMode,
                    UpdatedBy = CurrentUserId
                };

                await _service.UpdateInvoiceAsync(updateDto);
                TempData["SuccessMessage"] = "Invoice updated successfully!";
            }
            else
            {
                // Create new invoice (existing logic)
                await _service.CreateInvoiceAsync(new CreateInvoiceDto
                {
                    CompanyId = CurrentCompanyId,
                    SubcontractorId = model.SubcontractorId,
                    ProductId = model.ProductId,
                    InvoiceNo = model.InvoiceNo,
                    InvoiceDate = model.InvoiceDate,
                    UnitAmount = model.UnitAmount,
                    Quantity = model.Quantity,
                    TotalAmount = model.TotalAmount,
                    CommissionPercentage = model.CommissionPercentage,
                    CommissionAmount = model.CommissionAmount,
                    PaymentMode = model.PaymentMode,
                    CreatedBy = CurrentUserId
                });
                TempData["SuccessMessage"] = "Invoice added successfully!";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoice(int id)
        {
            int companyId = CurrentCompanyId;
            var invoice = (await _service.GetInvoicesAsync(companyId, id)).FirstOrDefault();
            if (invoice == null) return NotFound();

            // Get all subcontractors and products for dropdown
            var subcontractors = await _subContractorService.GetAllSubContractorAsync(companyId);
            var products = await _productService.GetProductsAsync(companyId);

            // Find IDs based on name
            int subcontractorId = subcontractors.FirstOrDefault(s => s.Name == invoice.R_subcontractorName)?.Id ?? 0;
            int productId = products.FirstOrDefault(p => p.R_description == invoice.R_productName)?.R_id ?? 0;


            var model = new InvoiceViewModel
            {
                Id = invoice.R_id,
                InvoiceNo = invoice.R_invoiceNo,
                CompanyId = invoice.R_companyId,
                SubContractorId = subcontractorId,
                ProductName = invoice.R_productName,
                ProductId = productId,
                InvoiceDate = invoice.R_invoiceDate,
                UnitAmount = invoice.R_unitAmount,
                Quantity = invoice.R_quantity,
                TotalAmount = invoice.R_totalAmount,
                CommissionPercentage = invoice.R_commissionPercentage,
                CommissionAmount = invoice.R_commissionAmount,
                InvoiceType = invoice.R_invoiceType,
            };

            return Json(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInvoice(int id, int companyId, int updatedBy)
        {
            companyId = CurrentCompanyId;
            updatedBy = CurrentUserId;
            try
            {
                await _service.DeleteInvoiceAsync(id, updatedBy, companyId);
                TempData["SuccessMessage"] = "Invoice deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete invoice. " + ex.Message;
            }

            return RedirectToAction(nameof(Index), new { companyId });
        }

    }
}
