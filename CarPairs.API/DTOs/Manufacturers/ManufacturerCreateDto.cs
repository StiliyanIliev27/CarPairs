using System.ComponentModel.DataAnnotations;

namespace CarPairs.API.DTOs.Manufacturers
{
    public class ManufacturerCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [Range(1800, 9999)]
        public int FoundedYear { get; set; }

        [MaxLength(200)]
        public string? Website { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
