using CarPairs.API.Extensions;
using CarPairs.Core;
using CarPairs.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarPairs.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrganizationService _organizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            IOrganizationService organizationService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _organizationService = organizationService;
            _userManager = userManager;
        }

        private bool IsAdmin()
        {
            var role = User.GetUserRole();
            return role == UserRole.Admin;
        }

        [HttpGet("stats")]
        public async Task<ActionResult> GetStats(CancellationToken ct)
        {
            if (!IsAdmin()) return Forbid();

            var stats = new
            {
                TotalOrganizations = await _context.Organizations.CountAsync(ct),
                ActiveOrganizations = await _context.Organizations.CountAsync(o => o.IsActive, ct),
                TotalUsers = await _context.Users.CountAsync(ct),
                TotalManufacturers = await _context.Manufacturers.CountAsync(ct),
                TotalParts = await _context.Parts.CountAsync(ct),
                TotalCategories = await _context.Categories.CountAsync(ct)
            };

            return Ok(stats);
        }

        [HttpGet("organizations")]
        public async Task<ActionResult> GetAllOrganizations(CancellationToken ct)
        {
            if (!IsAdmin()) return Forbid();

            var orgs = await _context.Organizations
                .OrderBy(o => o.Name)
                .Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.Description,
                    o.ContactEmail,
                    o.PhoneNumber,
                    o.IsActive,
                    o.CreatedAt,
                    UserCount = o.Users.Count(),
                    ManufacturerCount = o.Manufacturers.Count(),
                    PartCount = o.Parts.Count()
                })
                .ToListAsync(ct);

            return Ok(orgs);
        }

        [HttpGet("organizations/{id:int}")]
        public async Task<ActionResult> GetOrganization(int id, CancellationToken ct)
        {
            if (!IsAdmin()) return Forbid();

            var org = await _context.Organizations
                .Where(o => o.Id == id)
                .Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.Description,
                    o.ContactEmail,
                    o.PhoneNumber,
                    o.IsActive,
                    o.CreatedAt,
                    UserCount = o.Users.Count(),
                    ManufacturerCount = o.Manufacturers.Count(),
                    PartCount = o.Parts.Count()
                })
                .FirstOrDefaultAsync(ct);

            if (org == null) return NotFound();
            return Ok(org);
        }

        [HttpPost("organizations")]
        public async Task<ActionResult> CreateOrganization([FromBody] OrganizationDto dto, CancellationToken ct)
        {
            if (!IsAdmin()) return Forbid();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new Organization
            {
                Name = dto.Name,
                Description = dto.Description,
                ContactEmail = dto.ContactEmail,
                PhoneNumber = dto.PhoneNumber,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            var id = await _organizationService.CreateAsync(entity, ct);
            return CreatedAtAction(nameof(GetOrganization), new { id }, new { id });
        }

        [HttpPut("organizations/{id:int}")]
        public async Task<ActionResult> UpdateOrganization(int id, [FromBody] OrganizationDto dto, CancellationToken ct)
        {
            if (!IsAdmin()) return Forbid();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _organizationService.GetByIdAsync(id, ct);
            if (existing == null) return NotFound();

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.ContactEmail = dto.ContactEmail;
            existing.PhoneNumber = dto.PhoneNumber;
            existing.IsActive = dto.IsActive;

            await _organizationService.UpdateAsync(existing, ct);
            return NoContent();
        }

        [HttpDelete("organizations/{id:int}")]
        public async Task<ActionResult> DeleteOrganization(int id, CancellationToken ct)
        {
            if (!IsAdmin()) return Forbid();

            var deleted = await _organizationService.DeleteAsync(id, ct);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("users")]
        public async Task<ActionResult> GetAllUsers(CancellationToken ct)
        {
            if (!IsAdmin()) return Forbid();

            var users = await _context.Users
                .Include(u => u.Organization)
                .OrderBy(u => u.Email)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.UserName,
                    Role = u.Role.ToString(),
                    u.OrganizationId,
                    OrganizationName = u.Organization != null ? u.Organization.Name : null,
                    u.IsActive,
                    u.CreatedAt
                })
                .ToListAsync(ct);

            return Ok(users);
        }

        [HttpPut("users/{id}")]
        public async Task<ActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto, CancellationToken ct)
        {
            if (!IsAdmin()) return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.Role = dto.Role;
            user.OrganizationId = dto.OrganizationId;
            user.IsActive = dto.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser(string id, CancellationToken ct)
        {
            if (!IsAdmin()) return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }
    }

    public class OrganizationDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ContactEmail { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateUserDto
    {
        public UserRole Role { get; set; }
        public int? OrganizationId { get; set; }
        public bool IsActive { get; set; }
    }
}
