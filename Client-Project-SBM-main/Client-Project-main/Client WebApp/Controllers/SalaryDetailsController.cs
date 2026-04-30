using Client.Application.Features.SalaryDetails.Dtos;
using Client.Domain.Models;
using Client_WebApp.Middleware;
using Client_WebApp.Models;
using Client_WebApp.Services;
using Client_WebApp.Services.Master;
using Microsoft.AspNetCore.Mvc;

namespace Client_WebApp.Controllers
{
    public class SalaryDetailsController : BaseController
    {
        private readonly SalaryDetailsService _service;
        private readonly BankService _bankService;

        public SalaryDetailsController(SalaryDetailsService service, BankService bankService)
        {
            _service = service;
            _bankService = bankService;
        }

        public async Task<IActionResult> Index(string? searchText = null)
        {
            try
            {
                //if (!AccessHelper.HasAccess(User, "SalaryDetails", "View"))
                //    return Forbid();

                var companyId = CurrentCompanyId;
                var salaryDetails = await _service.GetAllAsync(companyId);
                var banksList = await _bankService.GetAllBanksAsync();

                var bankList = banksList
                .Select(sc => new Bank
                {
                    Id = sc.R_id,
                    Name = sc.R_bankName
                })
                .ToList();

                var viewModel = new SalaryDetailsViewModel
                {
                    CompanyId = companyId,
                    SalaryDetailList = salaryDetails,
                    Banks = bankList
                };

                ViewData["searchText"] = searchText;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load data. " + ex.Message;
                return View(new SalaryDetailsViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(SalaryDetailsViewModel model)
        {
            //if (model.SalaryDetails.Id > 0)
            //{
            //    if (!AccessHelper.HasAccess(User, "SalaryDetails", "Edit"))
            //        return Forbid();
            //}
            //else
            //{
            //    if (!AccessHelper.HasAccess(User, "SalaryDetails", "Create"))
            //        return Forbid();
            //}

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed.";
                return RedirectToAction("Index");
            }

            try
            {
                if (model.SalaryDetails.Id > 0)
                {
                    var updateDto = new UpdateSalaryDetailsDto
                    {
                        Id = model.SalaryDetails.Id,
                        ToliNo = model.SalaryDetails.ToliNo,
                        Amount = model.SalaryDetails.Amount,
                        //Quantity = model.SalaryDetails.Quantity,
                        Date = model.SalaryDetails.Date,
                        CompanyId = CurrentCompanyId,
                        BankId = model.SalaryDetails.BankId,
                        UpdatedBy = CurrentUserId
                    };
                    await _service.UpdateAsync(updateDto);
                    TempData["SuccessMessage"] = "Salary Details updated successfully!";
                }
                else
                {
                    var createDto = new CreateSalaryDetailsDto
                    {
                        ToliNo = model.SalaryDetails.ToliNo,
                        Amount = model.SalaryDetails.Amount,
                        //Quantity = model.SalaryDetails.Quantity,
                        Date = model.SalaryDetails.Date,
                        CompanyId = CurrentCompanyId,
                        BankId = model.SalaryDetails.BankId,
                        CreatedBy = CurrentUserId
                    };
                    await _service.InsertAsync(createDto);
                    TempData["SuccessMessage"] = "Salary Details added successfully!";
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

                //if (!AccessHelper.HasAccess(User, "SalaryDetails", "Delete"))
                //    return Forbid();


                await _service.DeleteAsync(id, CurrentUserId, CurrentCompanyId);
                TempData["SuccessMessage"] = "Salary Details deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete Salary Details: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetSalaryDetails(int id)
        {
            try
            {
                //if (!AccessHelper.HasAccess(User, "SalaryDetails", "View"))
                //    return Forbid();


                var entities = await _service.GetAllAsync(CurrentCompanyId, id);
                var entity = entities.FirstOrDefault();
                if (entity == null) return NotFound(new { message = "Salary Details not found." });

                return Json(new
                {
                    id = entity.R_id,
                    toliNo = entity.R_toliNo,
                    amount = entity.R_amount,
                    //quantity = entity.R_quantity,
                    date = entity.R_date,
                    companyId = entity.R_companyId,
                    bankId = entity.R_bankId,
                    bankName = entity.R_bankName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch Salary Details data: " + ex.Message });
            }
        }
    }
}
