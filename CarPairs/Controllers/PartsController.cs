using CarPairs.API.DTOs.Parts;
using Microsoft.AspNetCore.Mvc;

namespace CarPairs.API
{
    public class PartsController : Controller
    {
        private readonly IPartApiService _service;

        public PartsController(IPartApiService service)
        {
            _service = service;
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePartDto dto)
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
            var part = await _service.GetByIdAsync(id);

            if (part == null)
                return NotFound();

            var dto = new UpdatePartDto
            {
                Id = part.Id,
                Name = part.Name,
                Price = part.Price,
                StockQuantity = part.StockQuantity
                // ManufacturerId and CategoryId must be added if needed
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePartDto dto)
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
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}