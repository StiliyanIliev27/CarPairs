using System.ComponentModel.DataAnnotations;

namespace CarPairs.Models
{
    public class Category
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}
