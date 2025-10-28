using Client.Application.Features.User.Dtos;
using Client.Application.Interfaces;
using Client_WebApp.Middleware;
using Client_WebApp.Models.Master;
using Client_WebApp.Services;
using Client_WebApp.Services.Config;
using Client_WebApp.Services.Master;
using Microsoft.AspNetCore.Mvc;

namespace Client_WebApp.Controllers.Master
{
    public class UserController : BaseController
    {
        private readonly UserService _service;
        private readonly RoleService _roleService; 

        public UserController(UserService service, RoleService roleService)
        {
            _service = service;
            _roleService = roleService;
        }

        public async Task<IActionResult> Index(string? searchText = null)
        {
            try
            {
                if (!AccessHelper.HasAccess(User, "USER", "View"))
                    return Forbid();

                var companyId = CurrentCompanyId;
                var users = await _service.GetAllUsersAsync(companyId, null, searchText);
                var roles = await _roleService.GetRoleAsync();

                var viewModel = new UserViewModel
                {
                    CompanyId = companyId,
                    Users = users,
                    Roles = roles
                };

                ViewData["searchText"] = searchText;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load users. " + ex.Message;
                return View(new UserViewModel());
            }
        }

        //Updated sp_sbs_userMaster_insert sp in insert query replace this "SHA2(p_password, 512)"
        //to "UNHEX(SHA2(p_password, 512))" and also update input password parameter to "IN p_password varbinary(64)"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(User model)
        {
            if (model.Id > 0)
            {
                if (!AccessHelper.HasAccess(User, "USER", "Edit"))
                    return Forbid();
            }
            else
            {
                if (!AccessHelper.HasAccess(User, "USER", "Create"))
                    return Forbid();
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed.";
                return RedirectToAction("Index");
            }

            try
            {
                if (model.Id > 0)
                {
                    var updateDto = new UpdateUserDto
                    {
                        Id = model.Id,
                        RoleMasterId = model.RoleMasterId,
                        Username = model.Username,
                        Email = model.Email,
                        CompanyID = model.CompanyId,
                        UpdatedBy = CurrentUserId
                    };
                    await _service.UpdateUserAsync(updateDto);
                    TempData["SuccessMessage"] = "User updated successfully!";
                }
                else
                {
                    var createDto = new CreateUserDto
                    {
                        RoleMasterId = model.RoleMasterId,
                        Username = model.Username,
                        Email = model.Email,
                        Password = model.Password,
                        CompanyID = model.CompanyId,
                        CreatedBy = CurrentUserId
                    };
                    await _service.CreateUserAsync(createDto);
                    TempData["SuccessMessage"] = "User added successfully!";
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (!AccessHelper.HasAccess(User, "USER", "Delete"))
                    return Forbid();

                await _service.DeleteUserAsync(id, CurrentUserId, CurrentCompanyId);
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete user. " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                if (!AccessHelper.HasAccess(User, "USER", "View"))
                    return Forbid();

                var users = await _service.GetAllUsersAsync(CurrentCompanyId, id);
                var user = users.FirstOrDefault();
                if (user == null) return NotFound(new { message = "User not found." });

                return Json(new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    roleMasterId = user.RoleMasterId,
                    companyId = user.CompanyID,
                    isActive = user.isActive
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch user data. " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(int id, int isActive)
        {
            try
            {
                var dto = new ToggleUserActiveDto { Id = id, IsActive = isActive };
                await _service.ToggleActiveAsync(dto, CurrentCompanyId);
                TempData["SuccessMessage"] = isActive == 1 ? "User activated successfully!" : "User deactivated successfully!";

                return Json(new { success = true, message = isActive == 1 ? "User activated successfully!" : "User deactivated successfully!" });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to update user status: " + ex.Message;
                return Json(new { success = false, message = "Failed to update user status: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ChangeUserPassword()
        {
            // Use currently logged-in user info
            var model = new ChangePasswordViewModel
            {
                Id = CurrentUserId,
                Username = CurrentUserName,
                Email = CurrentUserEmail
            };
            return PartialView("_ChangePasswordPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserPassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields correctly.";
                return RedirectToAction("Index");
            }

            try
            {
                await _service.ChangePasswordAsync(new()
                {
                    Id = model.Id,
                    Username = model.Username,
                    CurrentPassword = model.CurrentPassword,
                    NewPassword = model.NewPassword,
                    Email = model.Email
                });

                TempData["SuccessMessage"] = "Password changed successfully! Please log in again.";

                // Clear session or cookies (if applicable)
                HttpContext.Session.Clear();

                // Redirect to Logout (which will handle cleanup and redirect to login page)
                return RedirectToAction("Logout", "Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to change password. " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult SendEmail()
        {
            var model = new SendEmailViewModel();
            return PartialView("_SendEmailPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmail(SendEmailViewModel model, [FromServices] IEmailService emailService)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields correctly.";
                return RedirectToAction("Index");
            }

            try
            {
                await emailService.SendEmailAsync(model.ToEmail, model.Subject, model.Body);
                TempData["SuccessMessage"] = "Email sent successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to send email. " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
