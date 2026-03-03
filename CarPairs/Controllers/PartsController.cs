using CarPairs.API.DTOs.Parts;
using CarPairs.Web.Services.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarPairs.API
{
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
            var result = await _service.GetAllAsync();
            return View(result?.Data ?? new List<PartDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var part = await _service.GetByIdAsync(id);
            if (part == null)
                return NotFound();

            return View(part);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await FillManufacturersAndCategories();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePartDto dto)
        {
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
        public async Task<IActionResult> Edit(int id)
        {
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
        public async Task<IActionResult> Edit(int id, UpdatePartDto dto)
        {
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
        public async Task<IActionResult> Delete(int id)
        {
            var part = await _service.GetByIdAsync(id);

            if (part == null)
                return NotFound();

            return View(part);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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