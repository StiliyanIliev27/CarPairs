using CarPairs.Data;
using CarPairs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace CarPairs.Controllers
{
    public class PartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            var list = await _context.Parts
                .Include(p => p.Manufacturer)
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);
            return View(list);
        }

        public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
                return NotFound();

            var part = await _context.Parts
                .Include(p => p.Manufacturer)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

            if (part == null)
                return NotFound();

            return View(part);
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
        {
            await FillManufacturersAndCategories(ViewData, cancellationToken);
            return View(new Part { CreatedAt = DateTime.UtcNow });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,StockQuantity,ManufacturerId,CategoryId,CreatedAt")] Part part, CancellationToken cancellationToken = default)
        {
            if (part.CreatedAt == default)
                part.CreatedAt = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                _context.Add(part);
                await _context.SaveChangesAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }

            await FillManufacturersAndCategories(ViewData, cancellationToken);
            return View(part);
        }

        public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
                return NotFound();

            var part = await _context.Parts.FindAsync(id, cancellationToken);
            if (part == null)
                return NotFound();

            await FillManufacturersAndCategories(ViewData, cancellationToken);
            return View(part);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,StockQuantity,ManufacturerId,CategoryId,CreatedAt")] Part part, CancellationToken cancellationToken = default)
        {
            if (id != part.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(part);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PartExists(part.Id, cancellationToken))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await FillManufacturersAndCategories(ViewData, cancellationToken);
            return View(part);
        }

        public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
                return NotFound();

            var part = await _context.Parts
                .Include(p => p.Manufacturer)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

            if (part == null)
                return NotFound();

            return View(part);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
        {
            var part = await _context.Parts.FindAsync(id, cancellationToken);
            if (part != null)
            {
                _context.Parts.Remove(part);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PartExists(int id, CancellationToken cancellationToken)
        {
            return await _context.Parts.AnyAsync(e => e.Id == id, cancellationToken);
        }

        private async Task FillManufacturersAndCategories(ViewDataDictionary viewData, CancellationToken cancellationToken)
        {
            viewData["Manufacturers"] = new SelectList(
                await _context.Manufacturers.OrderBy(m => m.Name).ToListAsync(cancellationToken),
                "Id", "Name");
            viewData["Categories"] = new SelectList(
                await _context.Categories.OrderBy(c => c.Name).ToListAsync(cancellationToken),
                "Id", "Name");
        }
    }
}
