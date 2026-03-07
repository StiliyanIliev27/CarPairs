using CarPairs.API.Extensions;
using CarPairs.Core;
using CarPairs.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarPairs.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get categories lookup for the user's organization
        /// </summary>
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

        /// <summary>
        /// Get paged list of categories for the user's organization
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<PagedResult<Category>>> GetCategories(
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
            return Ok(result);
        }

        /// <summary>
        /// Get category by id
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<Category>> GetCategory(int id, CancellationToken cancellationToken)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid("Organization not found for user");

            var category = await _service.GetByIdAsync(orgId, id, cancellationToken);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// Create a category (Manager and Admin only)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateCategory([FromBody] Category dto, CancellationToken cancellationToken)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null)
                return Forbid("Organization not found for user");

            if (!User.CanCreateInOrganization(orgId))
                return Forbid("You don't have permission to create categories in this organization");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                OrganizationId = orgId.Value,
                ParentCategoryId = dto.ParentCategoryId
            };

            var newId = await _service.CreateAsync(orgId.Value, entity, cancellationToken);

            return CreatedAtAction(nameof(GetCategory), new { id = newId }, new { id = newId });
        }

        /// <summary>
        /// Update a category (Manager and Admin only)
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category dto, CancellationToken cancellationToken)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid("Organization not found for user");

            if (!User.CanEditInOrganization(orgId))
                return Forbid("You don't have permission to edit categories in this organization");

            if (id != dto.Id)
                return BadRequest("Id mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = new Category
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                OrganizationId = orgId ?? 0,
                ParentCategoryId = dto.ParentCategoryId
            };

            var updated = await _service.UpdateAsync(orgId, entity, cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Delete a category (Admin only)
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
        {
            var orgId = User.GetOrganizationId();
            if (orgId == null && !User.IsSystemAdmin())
                return Forbid("Organization not found for user");

            var role = User.GetUserRole();
            if (role != UserRole.Admin)
                return Forbid("Only admins can delete categories");

            var deleted = await _service.DeleteAsync(orgId, id, cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
