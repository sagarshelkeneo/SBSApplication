using Client.Application.Features.Role.Dtos;
using Client_WebApp.Models;
using Client_WebApp.Models.Config;
using Client_WebApp.Services.Config;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Client_WebApp.Controllers.Config
{
    public class RoleController : BaseController
    {
        private readonly RoleService _service;

        public RoleController(RoleService service)
        {
            _service = service;
        }

        // ✅ GET: Role List with optional search
        public async Task<IActionResult> Index()
        {
            try
            {
                int companyId = CurrentCompanyId;

                // Fetch all roles for the company
                var roles = await _service.GetRoleAsync(null);

                // Map RoleDto → RoleDetails
                var roleDetails = roles.Select(r => new RoleDetails
                {
                    Id = r.Id,
                    RoleName = r.RoleName,
                    Description = r.Description
                }).ToList();

                // Prepare ViewModel
                var viewModel = new RoleViewModel
                {
                    CompanyId = companyId,
                    RoleDtos = roleDetails
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load roles. " + ex.Message;
                return View(new RoleViewModel());
            }
        }


        // ✅ POST: Create or Edit Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(RoleDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed. Please check the input fields.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (model.Id > 0)
                {
                    // Update Role
                    var updateDto = new UpdateRoleDto
                    {
                        Id = model.Id,
                        //CompanyId = CurrentCompanyId,
                        RoleName = model.RoleName,
                        Description = model.Description,
                        UpdatedBy = CurrentUserId
                    };

                    await _service.UpdateRoleAsync(updateDto);
                    TempData["SuccessMessage"] = "Role updated successfully!";
                }
                else
                {
                    // Create Role
                    var createDto = new CreateRoleDto
                    {
                        //CompanyId = CurrentCompanyId,
                        RoleName = model.RoleName,
                        Description = model.Description,
                        CreatedBy = CurrentUserId
                    };

                    await _service.CreateRoleAsync(createDto);
                    TempData["SuccessMessage"] = "Role added successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Operation failed. " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ POST: Delete Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int companyId = CurrentCompanyId;
                int updatedBy = CurrentUserId;

                await _service.DeleteRoleAsync(id, updatedBy, companyId);
                TempData["SuccessMessage"] = "Role deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete role. " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ GET: Fetch Role by Id (for Edit/View modal)
        [HttpGet]
        public async Task<IActionResult> GetRole(int id)
        {
            try
            {
                int companyId = CurrentCompanyId;
                var roles = await _service.GetRoleAsync(id);
                var role = roles.FirstOrDefault();

                if (role == null)
                    return NotFound(new { message = "Role not found." });

                return Json(new
                {
                    id = role.Id,
                    roleName = role.RoleName,
                    description = role.Description
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch role data. " + ex.Message });
            }
        }
    }
}
