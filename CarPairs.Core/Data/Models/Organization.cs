using System.ComponentModel.DataAnnotations;

namespace CarPairs.Core
{
    public class Organization
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Description { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string ContactEmail { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<Manufacturer> Manufacturers { get; set; } = new List<Manufacturer>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Part> Parts { get; set; } = new List<Part>();
    }
}
