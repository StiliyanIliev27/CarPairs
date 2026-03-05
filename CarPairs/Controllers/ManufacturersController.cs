using CarPairs.API.DTOs.Manufacturers;
using CarPairs.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarPairs.Controllers
{
    public class ManufacturersController : Controller
    {
        private readonly IManufacturerApiService _service;

        public ManufacturersController(IManufacturerApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _service.GetAllAsync();
            return View(result?.Data ?? new List<ManufacturerReadDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var manufacturer = await _service.GetByIdAsync(id);
            if (manufacturer == null)
                return NotFound();

            return View(manufacturer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ManufacturerCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var success = await _service.CreateAsync(dto);
            if (!success)
                return View(dto);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
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
        public async Task<IActionResult> Edit(int id, ManufacturerUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            var success = await _service.UpdateAsync(id, dto);
            if (!success)
                return View(dto);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var manufacturer = await _service.GetByIdAsync(id);
            if (manufacturer == null)
                return NotFound();

            return View(manufacturer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
