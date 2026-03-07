using CarPairs.API.DTOs.Manufacturers;
using CarPairs.Web.Extensions;
using CarPairs.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarPairs.Controllers
{
    [Authorize]
    public class ManufacturersController : Controller
    {
        private readonly IManufacturerApiService _service;

        public ManufacturersController(IManufacturerApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid();

            var result = await _service.GetAllAsync();
            return View(result?.Data ?? new List<ManufacturerReadDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid();

            var manufacturer = await _service.GetByIdAsync(id);
            if (manufacturer == null)
                return NotFound();

            return View(manufacturer);
        }

        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null)
                return Forbid();

            if (!User.CanManageOrganization(orgId))
                return Forbid();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(ManufacturerCreateDto dto)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null)
                return Forbid();

            if (!User.CanManageOrganization(orgId))
                return Forbid();

            if (!ModelState.IsValid)
                return View(dto);

            var success = await _service.CreateAsync(dto);
            if (!success)
                return View(dto);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid();

            if (!User.CanManageOrganization(orgId))
                return Forbid();

            var manufacturer = await _service.GetByIdAsync(id);
            if (manufacturer == null)
                return NotFound();

            var dto = new ManufacturerUpdateDto
            {
                Id = manufacturer.Id,
                Name = manufacturer.Name,
                Country = manufacturer.Country,
                FoundedYear = manufacturer.FoundedYear,
                Website = manufacturer.Website,
                IsActive = manufacturer.IsActive
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, ManufacturerUpdateDto dto)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid();

            if (!User.CanManageOrganization(orgId))
                return Forbid();

            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            var success = await _service.UpdateAsync(id, dto);
            if (!success)
                return View(dto);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid();

            var role = User.GetUserRole();
            if (role != "Admin")
                return Forbid();

            var manufacturer = await _service.GetByIdAsync(id);
            if (manufacturer == null)
                return NotFound();

            return View(manufacturer);
        }

        [HttpPost]
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
    }
}
