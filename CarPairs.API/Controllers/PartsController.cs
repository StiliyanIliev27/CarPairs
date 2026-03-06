using CarPairs.API.DTOs.Parts;
using CarPairs.API.Extensions;
using CarPairs.Core;
using CarPairs.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarPairs.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartsController : ControllerBase
    {
        private readonly IPartService _service;
        private readonly IOrganizationService _organizationService;

        public PartsController(IPartService service, IOrganizationService organizationService)
        {
            _service = service;
            _organizationService = organizationService;
        }

        /// <summary>
        /// Get parts for the authenticated user's organization
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<PagedResult<PartDto>>> GetParts(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            CancellationToken cancellationToken = default)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid("Organization not found for user");

            if (!User.CanViewOrganization(orgId))
                return Forbid("You don't have access to this organization");

            var result = await _service.GetAllAsync(orgId, pageNumber, pageSize, search, cancellationToken);

            var dto = new PagedResult<PartDto>
            {
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                Data = result.Data.Select(MapToDto).ToList()
            };

            return Ok(dto);
        }

        /// <summary>
        /// Get a specific part by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<PartDto>> GetPart(int id, CancellationToken cancellationToken)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid("Organization not found for user");

            var part = await _service.GetByIdAsync(orgId, id, cancellationToken);

            if (part == null)
                return NotFound();

            return Ok(MapToDto(part));
        }

        /// <summary>
        /// Create a new part (Manager and Admin only)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreatePart(
            [FromBody] CreatePartDto dto,
            CancellationToken cancellationToken)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null)
                return Forbid("Organization not found for user");

            if (!User.CanCreateInOrganization(orgId))
                return Forbid("You don't have permission to create parts in this organization");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var part = new Part
            {
                Name = dto.Name,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ManufacturerId = dto.ManufacturerId,
                CategoryId = dto.CategoryId,
                CreatedAt = DateTime.UtcNow,
                OrganizationId = orgId.Value
            };

            var newId = await _service.CreateAsync(orgId.Value, part, cancellationToken);

            return CreatedAtAction(nameof(GetPart), new { id = newId }, new { id = newId });
        }

        /// <summary>
        /// Update an existing part (Manager and Admin only)
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdatePart(
            int id,
            [FromBody] UpdatePartDto dto,
            CancellationToken cancellationToken)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid("Organization not found for user");

            if (!User.CanEditInOrganization(orgId))
                return Forbid("You don't have permission to edit parts in this organization");

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
                CreatedAt = DateTime.UtcNow,
                OrganizationId = orgId ?? 0
            };

            var updated = await _service.UpdateAsync(orgId, part, cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Delete a part (Admin only)
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeletePart(
            int id,
            CancellationToken cancellationToken)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid("Organization not found for user");

            var role = User.GetUserRole();
            if (role != UserRole.Admin)
                return Forbid("Only admins can delete parts");

            var deleted = await _service.DeleteAsync(orgId, id, cancellationToken);

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