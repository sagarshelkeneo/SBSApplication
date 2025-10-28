using Client.Application.Features.Company.Dtos;
using Client.Application.Features.Product.Dtos;
using Client_WebApp.Middleware;
using Client_WebApp.Models;
using Client_WebApp.Models.Config;
using Client_WebApp.Models.Master;
using Client_WebApp.Services.Config;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Client_WebApp.Controllers.Config
{
    public class CompanyController : BaseController
    {
        private readonly CompanyService _service;

        public CompanyController(CompanyService service)
        {
            _service = service;
        }

        // GET: Company List with optional search
        public async Task<IActionResult> Index(string searchText = null)
        {
            try
            {
                //Check if user has View access for Company module
                if (!AccessHelper.HasAccess(User, "COMPANY", "View"))
                {
                    return RedirectToAction("UnauthorizedAccess", "Home");
                }

                int companyId = CurrentCompanyId;
                var companies = await _service.GetCompanyAsync(null);

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    companies = companies
                        .Where(c => c.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                var viewModel = new CompanyViewModel
                {
                    CompanyId = companyId,
                    CompanyDtos = companies
                };

                ViewData["searchText"] = searchText;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load companies. " + ex.Message;
                return View(new CompanyViewModel());
            }
        }

        // POST: Create or Edit Company
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(CompanyDto model)
        {
            // Check Create or Edit permission
            if (model.Id > 0)
            {
                if (!AccessHelper.HasAccess(User, "COMPANY", "Edit"))
                    return RedirectToAction("UnauthorizedAccess", "Home");
            }
            else
            {
                if (!AccessHelper.HasAccess(User, "COMPANY", "Create"))
                    return RedirectToAction("UnauthorizedAccess", "Home");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed. Please check the input fields.";
                return RedirectToAction("Index");
            }

            try
            {
                if (model.Id > 0)
                {
                    // Update existing company
                    var updateDto = new UpdateCompanyDto
                    {
                        Id = model.Id,
                        //CompanyId = CurrentCompanyId,
                        Name = model.Name,
                        Address = model.Address,
                        Phone = model.Phone,
                        Email = model.Email,
                        UpdatedBy = CurrentUserId
                    };

                    await _service.UpdateCompanyAsync(updateDto);
                    TempData["SuccessMessage"] = "Company updated successfully!";
                }
                else
                {
                    // Create new company
                    var createDto = new CreateCompanyDto
                    {
                        //CompanyId = CurrentCompanyId,
                        Name = model.Name,
                        Address = model.Address,
                        Phone = model.Phone,
                        Email = model.Email,
                        CreatedBy = CurrentUserId
                    };

                    await _service.CreateCompanyAsync(createDto);
                    TempData["SuccessMessage"] = "Company added successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Operation failed. " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: Delete Company
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Check Delete permission
                if (!AccessHelper.HasAccess(User, "COMPANY", "Delete"))
                {
                    return RedirectToAction("UnauthorizedAccess", "Home");
                }

                int companyId = CurrentCompanyId;
                int updatedBy = CurrentUserId;

                await _service.DeleteCompanyAsync(id, updatedBy, companyId);
                TempData["SuccessMessage"] = "Company deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete company. " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Fetch Company for modal view/edit
        [HttpGet]
        public async Task<IActionResult> GetCompany(int id)
        {
            try
            {
                // View check for fetching company data
                if (!AccessHelper.HasAccess(User, "COMPANY", "View"))
                {
                    return RedirectToAction("UnauthorizedAccess", "Home");
                }

                int companyId = CurrentCompanyId;
                var companies = await _service.GetCompanyAsync(id);
                var company = companies.FirstOrDefault();

                if (company == null)
                    return NotFound(new { message = "Company not found." });

                return Json(new
                {
                    id = company.Id,
                    name = company.Name,
                    address = company.Address,
                    phone = company.Phone,
                    email = company.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch company data. " + ex.Message });
            }
        }
    }
}

