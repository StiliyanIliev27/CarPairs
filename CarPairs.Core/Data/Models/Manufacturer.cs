using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarPairs.Core
{
    public class Manufacturer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [Range(1800, 9999)]
        public int FoundedYear { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

 
        [Required]
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public Organization? Organization { get; set; }
        public ICollection<Part> Parts { get; set; } = new List<Part>();
    }
}
