using System.ComponentModel.DataAnnotations;

namespace CarPairs.Core
{
    public class Manufacturer
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        public int? FoundedYear { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
