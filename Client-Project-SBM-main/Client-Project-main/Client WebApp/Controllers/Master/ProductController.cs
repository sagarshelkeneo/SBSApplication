using Client.Application.Features.Product.Dtos;
using Client_WebApp.Models.Master;
using Client_WebApp.Services.Master;
using Microsoft.AspNetCore.Mvc;

namespace Client_WebApp.Controllers.Master
{
    public class ProductController : BaseController
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string searchText = null)
        {
            try
            {
                var products = await _service.GetProductsAsync(CurrentCompanyId, null, searchText);

                var viewModel = new ProductViewModel
                {
                    CompanyId = CurrentCompanyId,
                    Products = products
                };

                ViewData["searchText"] = searchText;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load products. " + ex.Message;
                return View(new ProductViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(Product model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed.";
                return RedirectToAction("Index");
            }

            try
            {
                if (model.Id > 0)
                {
                    var updateDto = new UpdateProductDto
                    {
                        Id = model.Id,
                        Description = model.Description,
                        UnitPrice = model.UnitPrice,
                        CompanyId = model.CompanyId,
                        UpdatedBy = CurrentUserId
                    };

                    await _service.UpdateProductAsync(updateDto);
                    TempData["SuccessMessage"] = "Product updated successfully!";
                }
                else
                {
                    var createDto = new CreateProductDto
                    {
                        Description = model.Description,
                        UnitPrice = model.UnitPrice,
                        CompanyId = model.CompanyId,
                        CreatedBy = CurrentUserId
                    };

                    await _service.CreateProductAsync(createDto);
                    TempData["SuccessMessage"] = "Product added successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Operation failed. " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int companyId)
        {
            try
            {
                await _service.DeleteProductAsync(id, CurrentUserId, companyId);
                TempData["SuccessMessage"] = "Product deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete product. " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var products = await _service.GetProductsAsync(CurrentCompanyId, id, null);
                var product = products.FirstOrDefault();

                if (product == null)
                    return NotFound(new { message = "Product not found." });

                return Json(new
                {
                    id = product.R_id,
                    companyid = product.R_companyID,
                    description = product.R_description,
                    unitPrice = product.R_unitPrice
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch product data. " + ex.Message });
            }
        }
    }
}
