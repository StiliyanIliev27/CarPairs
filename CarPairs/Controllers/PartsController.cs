using CarPairs.API.DTOs.Parts;
using CarPairs.Web.Extensions;
using CarPairs.Web.Services.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarPairs.Controllers
{
    [Authorize]
    public class PartsController : Controller
    {
        private readonly IPartApiService _service;
        private readonly ILookupApiService _lookupService;

        public PartsController(IPartApiService service, ILookupApiService lookupService)
        {
            _service = service;
            _lookupService = lookupService;
        }

        public async Task<IActionResult> Index()
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid();

            var result = await _service.GetAllAsync();
            return View(result?.Data ?? new List<PartDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid();

            var part = await _service.GetByIdAsync(id);
            if (part == null)
                return NotFound();

            return View(part);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create()
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null)
                return Forbid();

            if (!User.CanManageOrganization(orgId))
                return Forbid();

            await FillManufacturersAndCategories();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(CreatePartDto dto)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null)
                return Forbid();

            if (!User.CanManageOrganization(orgId))
                return Forbid();

            if (!ModelState.IsValid)
            {
                await FillManufacturersAndCategories(dto.ManufacturerId, dto.CategoryId);
                return View(dto);
            }

            var success = await _service.CreateAsync(dto);

            if (!success)
            {
                await FillManufacturersAndCategories(dto.ManufacturerId, dto.CategoryId);
                return View(dto);
            }        

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid();

            if (!User.CanManageOrganization(orgId))
                return Forbid();

            var part = await _service.GetByIdAsync(id);

            if (part == null)
                return NotFound();

            var dto = new UpdatePartDto
            {
                Id = part.Id,
                Name = part.Name,
                Price = part.Price,
                StockQuantity = part.StockQuantity,
                ManufacturerId = part.ManufacturerId,
                CategoryId = part.CategoryId
            };

            await FillManufacturersAndCategories(dto.ManufacturerId, dto.CategoryId);

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, UpdatePartDto dto)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid();

            if (!User.CanManageOrganization(orgId))
                return Forbid();

            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await FillManufacturersAndCategories(dto.ManufacturerId, dto.CategoryId);
                return View(dto);
            }

            var success = await _service.UpdateAsync(id, dto);

            if (!success)
            {
                await FillManufacturersAndCategories(dto.ManufacturerId, dto.CategoryId);
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid();

            var role = User.GetUserRole();
            if (role != "Admin")
                return Forbid();

            var part = await _service.GetByIdAsync(id);

            if (part == null)
                return NotFound();

            return View(part);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid();

            var role = User.GetUserRole();
            if (role != "Admin")
                return Forbid();

            var success = await _service.DeleteAsync(id);

            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        private async Task FillManufacturersAndCategories(int? selectedManufacturerId = null, int? selectedCategoryId = null)
        {
            var manufacturers = await _lookupService.GetManufacturersAsync() ?? new();
            var categories = await _lookupService.GetCategoriesAsync() ?? new();

            ViewData["Manufacturers"] = new SelectList(
                manufacturers,
                "Id",
                "Name",
                selectedManufacturerId);

            ViewData["Categories"] = new SelectList(
                categories,
                "Id",
                "Name",
                selectedCategoryId);
        }
    }
}