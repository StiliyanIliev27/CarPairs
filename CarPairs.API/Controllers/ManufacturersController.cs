using CarPairs.API.DTOs.Manufacturers;
using CarPairs.API.Extensions;
using CarPairs.Core;
using CarPairs.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarPairs.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManufacturersController : ControllerBase
    {
        private readonly IManufacturerService _service;

        public ManufacturersController(IManufacturerService service)
        {
            _service = service;
        }

        [HttpGet("lookup")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SimpleLookupDto>>> GetLookup(CancellationToken cancellationToken)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid("Organization not found for user");

            var result = await _service.GetLookupAsync(orgId, cancellationToken);
            return Ok(result);
        }

 
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<PagedResult<ManufacturerReadDto>>> GetManufacturers(
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid("Organization not found for user");

            if (!User.CanViewOrganization(orgId))
                return Forbid("You don't have access to this organization");

            var result = await _service.GetAllAsync(orgId, pageNumber, pageSize, cancellationToken);

            var dto = new PagedResult<ManufacturerReadDto>
            {
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                Data = result.Data.Select(MapToReadDto).ToList()
            };

            return Ok(dto);
        }

     
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ManufacturerReadDto>> GetManufacturer(int id, CancellationToken cancellationToken)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid("Organization not found for user");

            var manufacturer = await _service.GetByIdAsync(orgId, id, cancellationToken);

            if (manufacturer == null)
                return NotFound();

            return Ok(MapToReadDto(manufacturer));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateManufacturer([FromBody] ManufacturerCreateDto dto, CancellationToken cancellationToken)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null)
                return Forbid("Organization not found for user");

            if (!User.CanCreateInOrganization(orgId))
                return Forbid("You don't have permission to create manufacturers in this organization");

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
                CreatedAt = DateTime.UtcNow,
                OrganizationId = orgId.Value
            };

            var newId = await _service.CreateAsync(orgId.Value, entity, cancellationToken);

            return CreatedAtAction(nameof(GetManufacturer), new { id = newId }, new { id = newId });
        }

     
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateManufacturer(int id, [FromBody] ManufacturerUpdateDto dto, CancellationToken cancellationToken)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid("Organization not found for user");

            if (!User.CanEditInOrganization(orgId))
                return Forbid("You don't have permission to edit manufacturers in this organization");

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
                CreatedAt = DateTime.UtcNow,
                OrganizationId = orgId ?? 0
            };

            var updated = await _service.UpdateAsync(orgId, entity, cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteManufacturer(int id, CancellationToken cancellationToken)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid("Organization not found for user");

            var role = User.GetUserRole();
            if (role != UserRole.Admin)
                return Forbid("Only admins can delete manufacturers");

            var deleted = await _service.DeleteAsync(orgId, id, cancellationToken);

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
