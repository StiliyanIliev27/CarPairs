using CarPairs.API.DTOs.Parts;
using CarPairs.Core;
using CarPairs.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarPairs.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] // Enable after JWT is configured
    public class PartsController : ControllerBase
    {
        private readonly IPartService _service;

        public PartsController(IPartService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous] // optional until JWT ready
        public async Task<ActionResult<PagedResult<PartDto>>> GetParts(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize, search, cancellationToken);

            var dto = new PagedResult<PartDto>
            {
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                Data = result.Data.Select(MapToDto).ToList()
            };

            return Ok(dto);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<PartDto>> GetPart(int id, CancellationToken cancellationToken)
        {
            var part = await _service.GetByIdAsync(id, cancellationToken);

            if (part == null)
                return NotFound();

            return Ok(MapToDto(part));
        }

        [HttpPost]
        public async Task<ActionResult> CreatePart(
            [FromBody] CreatePartDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var part = new Part
            {
                Name = dto.Name,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ManufacturerId = dto.ManufacturerId,
                CategoryId = dto.CategoryId,
                CreatedAt = DateTime.Now
            };

            var newId = await _service.CreateAsync(part, cancellationToken);

            return CreatedAtAction(nameof(GetPart), new { id = newId }, new { id = newId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePart(
            int id,
            [FromBody] UpdatePartDto dto,
            CancellationToken cancellationToken)
        {
            if (id != dto.Id)
                return BadRequest("Id mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var part = new Part
            {
                Id = dto.Id,
                Name = dto.Name,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ManufacturerId = dto.ManufacturerId,
                CategoryId = dto.CategoryId,
                CreatedAt = DateTime.Now
            };

            var updated = await _service.UpdateAsync(part, cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePart(
            int id,
            CancellationToken cancellationToken)
        {
            var deleted = await _service.DeleteAsync(id, cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        private static PartDto MapToDto(Part p)
        {
            return new PartDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ManufacturerName = p.Manufacturer?.Name ?? string.Empty,
                CategoryName = p.Category?.Name ?? string.Empty,
                CreatedAt = p.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                ManufacturerId = p.ManufacturerId,
                CategoryId = p.CategoryId
            };
        }
    }
}