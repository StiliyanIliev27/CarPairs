using CarPairs.Core;

namespace CarPairs.Web.Services.Interfaces
{
    public interface IAdminApiService
    {
        // Dashboard
        Task<AdminStatsDto?> GetStatsAsync();

        // Organizations
        Task<List<AdminOrganizationDto>?> GetOrganizationsAsync();
        Task<AdminOrganizationDto?> GetOrganizationAsync(int id);
        Task<bool> CreateOrganizationAsync(OrganizationFormDto dto);
        Task<bool> UpdateOrganizationAsync(int id, OrganizationFormDto dto);
        Task<bool> DeleteOrganizationAsync(int id);

        // Users
        Task<List<AdminUserDto>?> GetUsersAsync();
        Task<bool> UpdateUserAsync(string id, UpdateUserFormDto dto);
        Task<bool> DeleteUserAsync(string id);
    }

    // ──── DTOs ───────────────────────────────────────────
    public class AdminStatsDto
    {
        public int TotalOrganizations { get; set; }
        public int ActiveOrganizations { get; set; }
        public int TotalUsers { get; set; }
        public int TotalManufacturers { get; set; }
        public int TotalParts { get; set; }
        public int TotalCategories { get; set; }
    }

    public class AdminOrganizationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ContactEmail { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserCount { get; set; }
        public int ManufacturerCount { get; set; }
        public int PartCount { get; set; }
    }

    public class OrganizationFormDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ContactEmail { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class AdminUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string Role { get; set; } = string.Empty;
        public int? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateUserFormDto
    {
        public UserRole Role { get; set; }
        public int? OrganizationId { get; set; }
        public bool IsActive { get; set; }
    }
}
