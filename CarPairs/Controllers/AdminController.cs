using CarPairs.Core;
using CarPairs.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarPairs.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminApiService _adminService;

        public AdminController(IAdminApiService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _adminService.GetStatsAsync();
            return View(stats ?? new AdminStatsDto());
        }

        public async Task<IActionResult> Organizations()
        {
            var orgs = await _adminService.GetOrganizationsAsync();
            return View(orgs ?? new List<AdminOrganizationDto>());
        }

        public IActionResult CreateOrganization()
        {
            return View(new OrganizationFormDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrganization(OrganizationFormDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var success = await _adminService.CreateOrganizationAsync(dto);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to create organization.");
                return View(dto);
            }

            return RedirectToAction(nameof(Organizations));
        }

        public async Task<IActionResult> EditOrganization(int id)
        {
            var org = await _adminService.GetOrganizationAsync(id);
            if (org == null) return NotFound();

            var dto = new OrganizationFormDto
            {
                Name = org.Name,
                Description = org.Description,
                ContactEmail = org.ContactEmail,
                PhoneNumber = org.PhoneNumber,
                IsActive = org.IsActive
            };

            ViewBag.OrganizationId = id;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrganization(int id, OrganizationFormDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.OrganizationId = id;
                return View(dto);
            }

            var success = await _adminService.UpdateOrganizationAsync(id, dto);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to update organization.");
                ViewBag.OrganizationId = id;
                return View(dto);
            }

            return RedirectToAction(nameof(Organizations));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrganization(int id)
        {
            await _adminService.DeleteOrganizationAsync(id);
            return RedirectToAction(nameof(Organizations));
        }

        public async Task<IActionResult> Users()
        {
            var users = await _adminService.GetUsersAsync();
            var orgs = await _adminService.GetOrganizationsAsync();

            ViewBag.Organizations = orgs ?? new List<AdminOrganizationDto>();
            return View(users ?? new List<AdminUserDto>());
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var users = await _adminService.GetUsersAsync();
            var user = users?.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            var orgs = await _adminService.GetOrganizationsAsync();
            ViewBag.Organizations = orgs ?? new List<AdminOrganizationDto>();

            var dto = new UpdateUserFormDto
            {
                Role = Enum.Parse<UserRole>(user.Role),
                OrganizationId = user.OrganizationId,
                IsActive = user.IsActive
            };

            ViewBag.UserId = id;
            ViewBag.UserEmail = user.Email;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, UpdateUserFormDto dto)
        {
            if (!ModelState.IsValid)
            {
                var orgs = await _adminService.GetOrganizationsAsync();
                ViewBag.Organizations = orgs ?? new List<AdminOrganizationDto>();
                ViewBag.UserId = id;
                return View(dto);
            }

            var success = await _adminService.UpdateUserAsync(id, dto);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to update user.");
                var orgs = await _adminService.GetOrganizationsAsync();
                ViewBag.Organizations = orgs ?? new List<AdminOrganizationDto>();
                ViewBag.UserId = id;
                return View(dto);
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _adminService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Users));
        }
    }
}
