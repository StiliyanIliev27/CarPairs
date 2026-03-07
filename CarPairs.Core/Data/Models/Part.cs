using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarPairs.Core
{
    public class Part
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        [ForeignKey("Manufacturer")]
        public int ManufacturerId { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Foreign key to Organization for multi-tenancy
        /// </summary>
        [Required]
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Manufacturer? Manufacturer { get; set; }
        public Category? Category { get; set; }
        public Organization? Organization { get; set; }
    }
}
