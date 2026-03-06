using CarPairs.API.DTOs.Manufacturers;
using CarPairs.Core;
using CarPairs.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarPairs.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ManufacturersController : ControllerBase
    {
        private readonly IManufacturerService _service;

        public ManufacturersController(IManufacturerService service)
        {
            _service = service;
        }
        
        [HttpGet("lookup")]
        public async Task<ActionResult<IEnumerable<SimpleLookupDto>>> GetLookup(CancellationToken cancellationToken)
        {
            var result = await _service.GetLookupAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get paged list of manufacturers
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<ManufacturerReadDto>>> GetManufacturers(
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize, cancellationToken);

            var dto = new PagedResult<ManufacturerReadDto>
            {
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                Data = result.Data.Select(MapToReadDto).ToList()
            };

            return Ok(dto);
        }

        /// <summary>
        /// Get manufacturer by id
        /// </summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ManufacturerReadDto>> GetManufacturer(int id, CancellationToken cancellationToken)
        {
            var manufacturer = await _service.GetByIdAsync(id, cancellationToken);

            if (manufacturer == null)
                return NotFound();

            return Ok(MapToReadDto(manufacturer));
        }

        /// <summary>
        /// Create a manufacturer
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> CreateManufacturer([FromBody] ManufacturerCreateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.FoundedYear > DateTime.Now.Year)
                return BadRequest("FoundedYear cannot be in the future.");

            var entity = new Manufacturer
            {
                Name = dto.Name,
                Country = dto.Country,
                FoundedYear = dto.FoundedYear,
                Website = dto.Website,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };

            var newId = await _service.CreateAsync(entity, cancellationToken);

            return CreatedAtAction(nameof(GetManufacturer), new { id = newId }, new { id = newId });
        }

        /// <summary>
        /// Update a manufacturer
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateManufacturer(int id, [FromBody] ManufacturerUpdateDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id)
                return BadRequest("Id mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.FoundedYear > DateTime.Now.Year)
                return BadRequest("FoundedYear cannot be in the future.");

            var entity = new Manufacturer
            {
                Id = dto.Id,
                Name = dto.Name,
                Country = dto.Country,
                FoundedYear = dto.FoundedYear,
                Website = dto.Website,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };

            var updated = await _service.UpdateAsync(entity, cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Delete a manufacturer (Admin only)
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteManufacturer(int id, CancellationToken cancellationToken)
        {
            var deleted = await _service.DeleteAsync(id, cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        private static ManufacturerReadDto MapToReadDto(Manufacturer m)
        {
            return new ManufacturerReadDto
            {
                Id = m.Id,
                Name = m.Name,
                Country = m.Country,
                FoundedYear = m.FoundedYear,
                Website = m.Website,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt.ToString("dd-MM-yyyy HH:mm")
            };
        }
    }
}
