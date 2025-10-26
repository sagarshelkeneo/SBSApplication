using Client.Application.Features.AdditionalEntity.Dtos;
using Client.Domain.Models;
using Client_WebApp.Models;
using Client_WebApp.Services;
using Client_WebApp.Services.Master;
using Microsoft.AspNetCore.Mvc;

namespace Client_WebApp.Controllers
{
    public class AdditionalEntityController : BaseController
    {
        private readonly AdditionalEntityService _service;
        private readonly SubContractorService _subContractorService;

        public AdditionalEntityController(AdditionalEntityService service, SubContractorService subContractorService)
        {
            _service = service;
            _subContractorService = subContractorService;
        }

        public async Task<IActionResult> Index(string? searchText = null)
        {
            try
            {
                var companyId = CurrentCompanyId;
                var entities = await _service.GetAllAsync(companyId);
                var subContractorService = await _subContractorService.GetAllSubContractorAsync(companyId);

                var subContractors = subContractorService
                .Select(sc => new SubContractor
                {
                    Id = sc.Id,
                    Name = sc.Name
                })
                .ToList();

                var viewModel = new AdditionalEntityViewModel
                {
                    CompanyId = companyId,
                    AdditionalEntities = entities,
                    SubContractors = subContractors
                };

                ViewData["searchText"] = searchText;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load data. " + ex.Message;
                return View(new AdditionalEntityViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(AdditionalEntityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed.";
                return RedirectToAction("Index");
            }

            try
            {
                if (model.AdditionalEntity.Id > 0)
                {
                    var updateDto = new UpdateAdditionalEntityDto
                    {
                        Id = model.AdditionalEntity.Id,
                        Type = model.AdditionalEntity.Type,
                        Amount = model.AdditionalEntity.Amount,
                        Quantity = model.AdditionalEntity.Quantity,
                        Date = model.AdditionalEntity.Date,
                        CompanyId = CurrentCompanyId,
                        SubContractorId = model.AdditionalEntity.SubContractorId,
                        UpdatedBy = CurrentUserId
                    };
                    await _service.UpdateAsync(updateDto);
                    TempData["SuccessMessage"] = "Additional Entity updated successfully!";
                }
                else
                {
                    var createDto = new CreateAdditionalEntityDto
                    {
                        Type = model.AdditionalEntity.Type,
                        Amount = model.AdditionalEntity.Amount,
                        Quantity = model.AdditionalEntity.Quantity,
                        Date = model.AdditionalEntity.Date,
                        CompanyId = CurrentCompanyId,
                        SubContractorId = model.AdditionalEntity.SubContractorId,
                        CreatedBy = CurrentUserId
                    };
                    await _service.InsertAsync(createDto);
                    TempData["SuccessMessage"] = "Additional Entity added successfully!";
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
                await _service.DeleteAsync(id, CurrentUserId, CurrentCompanyId);
                TempData["SuccessMessage"] = "Additional Entity deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete Additional Entity: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetAdditionalEntity(int id)
        {
            try
            {
                var entities = await _service.GetAllAsync(CurrentCompanyId, id);
                var entity = entities.FirstOrDefault();
                if (entity == null) return NotFound(new { message = "Additional Entity not found." });

                return Json(new
                {
                    id = entity.R_id,
                    type = entity.R_type,
                    amount = entity.R_amount,
                    quantity = entity.R_quantity,
                    date = entity.R_date,
                    companyId = entity.R_companyId,
                    subContractorId = entity.R_subContractorId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch Additional Entity data: " + ex.Message });
            }
        }
    }
}
