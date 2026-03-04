using System.ComponentModel.DataAnnotations;

namespace CarPairs.API.DTOs.Manufacturers
{
    public class ManufacturerReadDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        public int FoundedYear { get; set; }

        [MaxLength(200)]
        public string? Website { get; set; }

        public bool IsActive { get; set; }

        public string CreatedAt { get; set; } = string.Empty;
    }
}
