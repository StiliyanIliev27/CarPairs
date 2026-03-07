using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarPairs.Core
{
    public class Category
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key to Organization for multi-tenancy
        /// </summary>
        [Required]
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public int? ParentCategoryId { get; set; }

        // Navigation properties
        public Organization? Organization { get; set; }
        public ICollection<Part> Parts { get; set; } = new List<Part>();
    }
}
