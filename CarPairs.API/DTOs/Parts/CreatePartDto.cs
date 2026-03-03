using System.ComponentModel.DataAnnotations;

namespace CarPairs.API.DTOs.Parts
{
    public class CreatePartDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required]
        public int ManufacturerId { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
