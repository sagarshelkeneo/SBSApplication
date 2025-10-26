using Client.Application.Features.Bank.Dtos;
using Client_WebApp.Models.Master;
using Client_WebApp.Services.Master;
using Microsoft.AspNetCore.Mvc;

namespace Client_WebApp.Controllers.Master
{
    public class BankController : BaseController
    {
        private readonly BankService _service;

        public BankController(BankService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string searchText = null)
        {
            try
            {
                var banks = await _service.GetAllBanksAsync();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    banks = banks
                        .Where(b => b.R_bankName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                var viewModel = new BankViewModel
                {
                    Banks = banks
                };

                ViewData["searchText"] = searchText;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load banks. " + ex.Message;
                return View(new BankViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(Bank model)
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
                    var updateDto = new UpdateBankMasterDto
                    {
                        Id = model.Id,
                        BankName = model.BankName,
                        Branch = model.BankBranch,
                        UpdatedBy = CurrentUserId
                    };
                    await _service.UpdateBankAsync(updateDto);
                    TempData["SuccessMessage"] = "Bank updated successfully!";
                }
                else
                {
                    var createDto = new CreateBankMasterDto
                    {
                        BankName = model.BankName,
                        Branch = model.BankBranch,
                        CreatedBy = CurrentUserId
                    };
                    await _service.CreateBankAsync(createDto);
                    TempData["SuccessMessage"] = "Bank added successfully!";
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
                await _service.DeleteBankAsync(id, CurrentUserId);
                TempData["SuccessMessage"] = "Bank deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete bank. " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetBank(int id)
        {
            try
            {
                var banks = await _service.GetAllBanksAsync(id);
                var bank = banks.FirstOrDefault();

                if (bank == null)
                    return NotFound(new { message = "Bank not found." });

                return Json(new
                {
                    id = bank.R_id,
                    bankName = bank.R_bankName,
                    bankBranch = bank.R_branch
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch bank data. " + ex.Message });
            }
        }
    }
}
