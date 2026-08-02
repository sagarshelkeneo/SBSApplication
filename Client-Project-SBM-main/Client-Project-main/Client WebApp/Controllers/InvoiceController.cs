using Client.Application.Features.Invoice.Dtos;
using Client_WebApp.Controllers;
using Client_WebApp.Middleware;
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

        [HttpGet("invoice", Name = "Index")]
        public async Task<IActionResult> Index(string? searchText, DateTime? FromDate, DateTime? ToDate)
        {
            if (!AccessHelper.HasAccess(User, "INVOICE", "View"))
                return Forbid();

            int companyId = CurrentCompanyId;
            // Fetch application DTOs from service
            List<Client.Application.Features.Invoice.Dtos.InvoiceDetailsDto> invoicesFromService =
                await _service.GetInvoicesAsync(false,companyId);

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
                        ((i.R_invoiceNo != null && i.R_invoiceNo.ToString().ToLower().Contains(searchText)))
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
                InvoiceType = i.R_invoiceType,
                GroupNumber  = i.R_GroupNumber,
                LRNumber = i.R_LRNumber,
                VehicleNumber = i.R_VehicleNumber

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
            if (model.Id > 0)
            {
                if (!AccessHelper.HasAccess(User, "INVOICE", "Edit"))
                    return Forbid();
            }
            else
            {
                if (!AccessHelper.HasAccess(User, "INVOICE", "Create"))
                    return Forbid();
            }

            // Remove per-row fields — validated client-side via LRItems[n].*
            ModelState.Remove(nameof(model.UnitAmount));
            ModelState.Remove(nameof(model.Quantity));
            ModelState.Remove(nameof(model.TotalAmount));

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please fill all required fields." });
            }

            if (model.Id > 0)
            {
                // ── BULK UPDATE ─────────────────────────────────────────────
                var lrRows = model.LRItems
                    ?.Where(r => r.UnitAmount > 0 && r.Quantity > 0)
                    .ToList();

                if (lrRows == null || lrRows.Count == 0)
                {
                    lrRows = new List<LRItemViewModel>
                    {
                        new LRItemViewModel
                        {
                            ProductId   = model.ProductId,
                            LRNumber    = model.LRNumber,
                            UnitAmount  = model.UnitAmount,
                            Quantity    = model.Quantity,
                            TotalAmount = model.TotalAmount
                        }
                    };
                }

                var updateBulkDto = new UpdateInvoiceBulkDto
                {
                    Id = model.Id,
                    CompanyId = CurrentCompanyId,
                    SubcontractorId = model.SubcontractorId,
                    ProductId = lrRows.FirstOrDefault()?.ProductId ?? model.ProductId,
                    InvoiceNo = model.InvoiceNo,
                    InvoiceDate = model.InvoiceDate,
                    CommissionPercentage = model.CommissionPercentage,
                    CommissionAmount = model.CommissionAmount,
                    PaymentMode = model.PaymentMode,
                    UpdatedBy = CurrentUserId,
                    GroupNumber = model.GroupNumber,
                    VehicleNumber = model.VehicleNumber,
                    Levi = model.Levi,
                    DocketNumber = model.DocketNumber,
                    TrollyQuantity = model.TrollyQuantity,
                    TrollyAmount = model.TrollyAmount,
                    LRItems = lrRows.Select(item => new LRItemDto
                    {
                        ProductId = item.ProductId,
                        LRNumber = item.LRNumber,
                        UnitAmount = item.UnitAmount,
                        Quantity = item.Quantity,
                        TotalAmount = item.TotalAmount
                    }).ToList()
                };
                
                
                await _service.UpdateInvoiceBulkAsync(updateBulkDto);
                //TempData["SuccessMessage"] = "Booking updated successfully!";
                TempData["SuccessMessage"] = $"Booking updated successfully! with ({lrRows.Count} product record(s) saved.)";
            }
            else
            {
                // ── CREATE — bulk insert of all LR rows ─────────────────────
                var lrRows = model.LRItems
                    ?.Where(r => r.UnitAmount > 0 && r.Quantity > 0)
                    .ToList();

                if (lrRows == null || lrRows.Count == 0)
                {
                    // Fallback: single-row from legacy hidden fields
                    lrRows = new List<LRItemViewModel>
                    {
                        new LRItemViewModel
                        {
                            ProductId =  model.ProductId,
                            LRNumber    = model.LRNumber,
                            UnitAmount  = model.UnitAmount,
                            Quantity    = model.Quantity,
                            TotalAmount = model.TotalAmount
                        }
                    };
                }

                var bulkDto = new CreateInvoiceBulkDto
                {
                    CompanyId            = CurrentCompanyId,
                    SubcontractorId      = model.SubcontractorId,
                    ProductId            = model.ProductId,
                    InvoiceNo            = model.InvoiceNo,
                    InvoiceDate          = model.InvoiceDate,
                    CommissionPercentage = model.CommissionPercentage,
                    CommissionAmount     = model.CommissionAmount,
                    PaymentMode          = model.PaymentMode,
                    CreatedBy            = CurrentUserId,
                    GroupNumber          = model.GroupNumber,
                    VehicleNumber        = model.VehicleNumber,
                    IsLeviApplicable     = false,
                    Levi                 = model.Levi,
                    DocketNumber         = model.DocketNumber,
                    TrollyQuantity       = model.TrollyQuantity,
                    TrollyAmount         = model.TrollyAmount,
                    LRItems              = lrRows.Select(item => new LRItemDto
                    {
                        ProductId = item.ProductId,
                        LRNumber    = item.LRNumber,
                        UnitAmount  = item.UnitAmount,
                        Quantity    = item.Quantity,
                        TotalAmount = item.TotalAmount
                    }).ToList()
                };

                await _service.CreateInvoiceBulkAsync(bulkDto);

                TempData["SuccessMessage"] = $"Booking added successfully! with ({lrRows.Count} product record(s) saved.)";
            }

            return Json(new { success = true, message = TempData["SuccessMessage"]?.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoice(int id, bool isLeviApplicable)
        {
            if (!AccessHelper.HasAccess(User, "INVOICE", "View"))
                return Forbid();

            int companyId = CurrentCompanyId;
            var invoice = (await _service.GetInvoicesAsync(isLeviApplicable, companyId, id)).FirstOrDefault();
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
                SubContractorName = invoice.R_subcontractorName,
                ProductName = invoice.R_productName,
                ProductId = productId,
                InvoiceDate = invoice.R_invoiceDate,
                UnitAmount = invoice.R_unitAmount,
                Quantity = invoice.R_quantity,
                TotalAmount = invoice.R_totalAmount,
                CommissionPercentage = invoice.R_commissionPercentage,
                CommissionAmount = invoice.R_commissionAmount,
                InvoiceType = invoice.R_invoiceType,
                GroupNumber = invoice.R_GroupNumber,
                LRNumber = invoice.R_LRNumber,
                VehicleNumber = invoice.R_VehicleNumber,
                IsLeviApplicable = invoice.R_IsLeviApplicable,
                Levi = invoice.R_Levi,
                DocketNumber = invoice.R_DocketNumber,
                TrollyQuantity = invoice.R_TrollyQuantity,
                TrollyAmount = invoice.R_TrollyAmount
            };

            return Json(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInvoice(int id, int companyId, int updatedBy)
        {
            try
            {
                if (!AccessHelper.HasAccess(User, "INVOICE", "Delete"))
                    return Forbid();

                await _service.DeleteInvoiceAsync(id, CurrentUserId, CurrentCompanyId, false);
                TempData["SuccessMessage"] = "Booking deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete booking. " + ex.Message;
            }

            return RedirectToAction(nameof(Index), new { companyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInvoiceData(int id, int companyId, int updatedBy)
        {
            try
            {
                if (!AccessHelper.HasAccess(User, "INVOICE", "Delete"))
                    return Forbid();

                await _service.DeleteInvoiceAsync(id, CurrentUserId, CurrentCompanyId, true);
                TempData["SuccessMessage"] = "Booking deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete booking. " + ex.Message;
            }

            return RedirectToAction(nameof(getInvoiceData), new { companyId });
        }



        [HttpGet("invoice/getInvoiceData", Name = "getInvoiceData")]
        public async Task<IActionResult> getInvoiceData(string? searchText, DateTime? FromDate, DateTime? ToDate)
        {
            if (!AccessHelper.HasAccess(User, "INVOICE", "View"))
                return Forbid();

            int companyId = CurrentCompanyId;
            // Fetch application DTOs from service
            List<Client.Application.Features.Invoice.Dtos.InvoiceDetailsDto> invoicesFromService =
                await _service.GetInvoicesAsync(true, companyId);

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
                InvoiceType = i.R_invoiceType,
                GroupNumber = i.R_GroupNumber,
                LRNumber = i.R_LRNumber,
                VehicleNumber = i.R_VehicleNumber,
                IsLeviApplicable = i.R_IsLeviApplicable,
                Levi = i.R_Levi,
                DocketNumber = i.R_DocketNumber,
                TrollyQuantity = i.R_TrollyQuantity,
                TrollyAmount = i.R_TrollyAmount,
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
        public async Task<IActionResult> CreateOrEditInvoice(AddInvoiceViewModel model)
        {
            if (model.Id > 0)
            {
                if (!AccessHelper.HasAccess(User, "INVOICE", "Edit"))
                    return Forbid();
            }
            else
            {
                if (!AccessHelper.HasAccess(User, "INVOICE", "Create"))
                    return Forbid();
            }

            // Remove per-row fields — validated client-side via LRItems[n].*
            ModelState.Remove(nameof(model.UnitAmount));
            ModelState.Remove(nameof(model.Quantity));
            ModelState.Remove(nameof(model.TotalAmount));

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please fill all required fields." });
            }

            if (model.Id > 0)
            {
                // ── BULK UPDATE ─────────────────────────────────────────────
                var lrRows = model.LRItems
                    ?.Where(r => r.UnitAmount > 0 && r.Quantity > 0)
                    .ToList();

                if (lrRows == null || lrRows.Count == 0)
                {
                    lrRows = new List<LRItemViewModel>
                    {
                        new LRItemViewModel
                        {
                            ProductId   = model.ProductId,
                            LRNumber    = model.LRNumber,
                            UnitAmount  = model.UnitAmount,
                            Quantity    = model.Quantity,
                            TotalAmount = model.TotalAmount
                        }
                    };
                }

                var updateBulkDto = new UpdateInvoiceBulkDto
                {
                    Id                   = model.Id,
                    CompanyId            = CurrentCompanyId,
                    SubcontractorId      = model.SubcontractorId,
                    ProductId            = lrRows.FirstOrDefault()?.ProductId ?? model.ProductId,
                    InvoiceNo            = model.InvoiceNo,
                    InvoiceDate          = model.InvoiceDate,
                    CommissionPercentage = model.CommissionPercentage,
                    CommissionAmount     = model.CommissionAmount,
                    PaymentMode          = model.PaymentMode,
                    UpdatedBy            = CurrentUserId,
                    GroupNumber          = model.GroupNumber,
                    VehicleNumber        = model.VehicleNumber,
                    IsLeviApplicable     = true,
                    Levi                 = model.Levi,
                    DocketNumber         = model.DocketNumber,
                    TrollyQuantity       = model.TrollyQuantity,
                    TrollyAmount         = model.TrollyAmount,
                    LRItems              = lrRows.Select(item => new LRItemDto
                    {
                        ProductId   = item.ProductId,
                        LRNumber    = item.LRNumber,
                        UnitAmount  = item.UnitAmount,
                        Quantity    = item.Quantity,
                        TotalAmount = item.TotalAmount
                    }).ToList()
                };

                await _service.UpdateInvoiceBulkAsync(updateBulkDto);
                TempData["SuccessMessage"] = $"Booking updated successfully! with ({lrRows.Count} product record(s) saved.)";
            }
            else
            {
                // ── CREATE — bulk insert of all LR rows ─────────────────────
                var lrRows = model.LRItems
                    ?.Where(r => r.UnitAmount > 0 && r.Quantity > 0)
                    .ToList();

                if (lrRows == null || lrRows.Count == 0)
                {
                    // Fallback: single-row from legacy hidden fields
                    lrRows = new List<LRItemViewModel>
                    {
                        new LRItemViewModel
                        {
                            ProductId   = model.ProductId,
                            LRNumber    = model.LRNumber,
                            UnitAmount  = model.UnitAmount,
                            Quantity    = model.Quantity,
                            TotalAmount = model.TotalAmount
                        }
                    };
                }

                var bulkDto = new CreateInvoiceBulkDto
                {
                    CompanyId            = CurrentCompanyId,
                    SubcontractorId      = model.SubcontractorId,
                    ProductId            = lrRows.FirstOrDefault()?.ProductId ?? model.ProductId,
                    InvoiceNo            = model.InvoiceNo,
                    InvoiceDate          = model.InvoiceDate,
                    CommissionPercentage = model.CommissionPercentage,
                    CommissionAmount     = model.CommissionAmount,
                    PaymentMode          = model.PaymentMode,
                    CreatedBy            = CurrentUserId,
                    GroupNumber          = model.GroupNumber,
                    VehicleNumber        = model.VehicleNumber,
                    IsLeviApplicable     = true,
                    Levi                 = model.Levi,
                    DocketNumber         = model.DocketNumber,
                    TrollyQuantity       = model.TrollyQuantity,
                    TrollyAmount         = model.TrollyAmount,
                    LRItems              = lrRows.Select(item => new LRItemDto
                    {
                        ProductId   = item.ProductId,
                        LRNumber    = item.LRNumber,
                        UnitAmount  = item.UnitAmount,
                        Quantity    = item.Quantity,
                        TotalAmount = item.TotalAmount
                    }).ToList()
                };

                await _service.CreateInvoiceBulkAsync(bulkDto);

                TempData["SuccessMessage"] = $"Booking added successfully! with ({lrRows.Count} product record(s) saved.)";
            }

            return Json(new { success = true, message = TempData["SuccessMessage"]?.ToString() });
        }

        [HttpGet("invoice/getInvoiceTransactionDetailsData", Name = "getInvoiceTransactionDetailsData")]
        public async Task<IActionResult> getInvoiceTransactionDetailsData(int id, int InvoiceId)
        {
            try
            {
                if (!AccessHelper.HasAccess(User, "INVOICE", "View"))
                    return Forbid();

                int companyId = CurrentCompanyId;
                // Fetch application DTOs from service
                List<Client.Application.Features.Invoice.Dtos.InvoiceTransactionDetailsDto> invoicesFromService =
                    await _service.GetInvoicesTransactionDetailsAsync(InvoiceId, id > 0 ? id : null);

                
                // Map to WebApp DTO
                var webInvoices = invoicesFromService.Select(i => new Client_WebApp.Models.InvoiceTransactionDetailsDto
                {
                    id = i.R_id,
                    invoiceid = i.R_invoiceid,
                    productId = i.R_productId,
                    productName = i.R_productName,
                    unitAmount = i.R_unitAmount,
                    quantity = i.R_quantity,
                    totalAmount = i.R_totalAmount,
                    LRNumber = i.R_LRNumber,
                    createdBy = i.createdBy,
                    createdAt = i.createdAt
                }).ToList();


                return Json(new { success = true, data = webInvoices });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, data = new List<Client_WebApp.Models.InvoiceTransactionDetailsDto>() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoices(
            int? invoiceId,
            int? subContractorId,
            DateTime? paymentDate,
            DateTime? fromDate,
            DateTime? toDate,
            bool isLeviApplicable = false)
        {
            try
            {
                var invoices = await _service.GetInvoiceAsPerContractorAsync(
                    invoiceId,
                    subContractorId,
                    paymentDate,
                    fromDate,
                    toDate,
                    isLeviApplicable
                );

                var webInvoices = invoices.Select(i => new Client_WebApp.Models.InvoiceDetailsDto
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
                    InvoiceType = i.R_invoiceType,
                    GroupNumber = i.R_GroupNumber,
                    LRNumber = i.R_LRNumber,
                    VehicleNumber = i.R_VehicleNumber,
                    IsLeviApplicable = i.R_IsLeviApplicable,
                    Levi = i.R_Levi,
                    DocketNumber = i.R_DocketNumber,
                    TrollyQuantity = i.R_TrollyQuantity,
                    TrollyAmount = i.R_TrollyAmount,
                }).ToList();


                return Json(new { success = true, data = webInvoices });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
