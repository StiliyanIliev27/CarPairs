using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarPairs.Core
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// The organization this user belongs to.
        /// Null for system admins with cross-org access.
        /// </summary>
        [ForeignKey("Organization")]
        public int? OrganizationId { get; set; }

        /// <summary>
        /// User's role within the system/organization
        /// </summary>
        [Required]
        public UserRole Role { get; set; } = UserRole.User;

        /// <summary>
        /// Account status
        /// </summary>
        [Required]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// When the user was created
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Organization? Organization { get; set; }
    }
}