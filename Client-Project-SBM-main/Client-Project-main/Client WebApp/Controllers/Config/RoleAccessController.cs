using Client.Application.Features.Role.Dtos;
using Client.Application.Features.RoleAccessControl.Dtos;
using Client_WebApp.Middleware;
using Client_WebApp.Models.Config;
using Client_WebApp.Services.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Client_WebApp.Controllers.Config
{
    public class RoleAccessController : BaseController
    {

        private readonly RoleService _role_service;

        private readonly RoleAccessService _service;

        public RoleAccessController(RoleAccessService service, RoleService role_service)
        {
            _service = service;
            _role_service = role_service;
        }
        public async Task<IActionResult> Index()
        {
            // Check if user has "View" access for Role Access module
            if (!AccessHelper.HasAccess(User, "ROLEACCESS", "View"))
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            var roles = await _role_service.GetRoleAsync(null);
            List<RoleDetails> rd = new List<RoleDetails>();

            foreach (var v in roles)
            {

                rd.Add(new RoleDetails()
                {
                    Id = v.Id,
                    RoleName = v.RoleName
                });
            }


            // Pass to the view
            return View(rd);

        }

        [HttpGet]
        public async Task<JsonResult> GetRoleAccessByUser(int userId)
        {
            // View permission check
            if (!AccessHelper.HasAccess(User, "ROLEACCESS", "View"))
            {
                return Json(new { success = false, message = "Unauthorized to view role access details." });
            }

            // Example data (replace with DB query)
            var data = await _service.GetRoleAccessByRoleIdAsync(userId);

            if (data == null || !data.Any())
            {
                return Json(new { success = false, message = "No role access found." });
            }

            // Return data properly serialized as JSON
            return Json(new
            {
                success = true,
                result = data
            });


        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoleAccess([FromBody] UpdateRoleAccessDto dto)
        {
            // Edit permission check
            if (!AccessHelper.HasAccess(User, "ROLEACCESS", "Edit"))
            {
                return Json(new { success = false, message = "You do not have permission to edit role access." });
            }

            if (dto == null)
                return Json(new { success = false, message = "No data received" });

            dto.UpdatedBy = CurrentUserId;
            var result = await _service.UpdateRoleAccessAsync(dto);

            return Json(new { success = result });

        }


    }


}

