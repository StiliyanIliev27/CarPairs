using System.ComponentModel.DataAnnotations;

namespace CarPairs.API.DTOs.Parts
{
    public class PartDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string ManufacturerName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }
}
