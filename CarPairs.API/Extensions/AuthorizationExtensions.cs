using System.Security.Claims;
using CarPairs.Core;

namespace CarPairs.API.Extensions
{
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Get the user's organization ID from claims
        /// </summary>
        public static int? GetOrganizationId(this ClaimsPrincipal user)
        {
            var orgClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (orgClaim == null)
                return null;

            // Try to get from custom claim first
            var customOrgClaim = user.FindFirst("OrganizationId");
            if (customOrgClaim != null && int.TryParse(customOrgClaim.Value, out var orgId))
                return orgId;

            return null;
        }

        /// <summary>
        /// Get the user's role
        /// </summary>
        public static UserRole? GetUserRole(this ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            if (roleClaim != null && Enum.TryParse<UserRole>(roleClaim.Value, out var role))
                return role;

            var customRoleClaim = user.FindFirst("UserRole");
            if (customRoleClaim != null && Enum.TryParse<UserRole>(customRoleClaim.Value, out var customRole))
                return customRole;

            return null;
        }

        /// <summary>
        /// Check if the user is a system admin (no organization ID)
        /// </summary>
        public static bool IsSystemAdmin(this ClaimsPrincipal user)
        {
            return user.GetOrganizationId() == null && user.GetUserRole() == UserRole.Admin;
        }

        /// <summary>
        /// Check if user can manage in the given organization
        /// </summary>
        public static bool CanManageOrganization(this ClaimsPrincipal user, int? organizationId)
        {
            if (user.IsSystemAdmin())
                return true;

            var userOrgId = user.GetOrganizationId();
            var role = user.GetUserRole();

            if (userOrgId != organizationId)
                return false;

            return role == UserRole.Admin || role == UserRole.Manager;
        }

        /// <summary>
        /// Check if user can view in the given organization
        /// </summary>
        public static bool CanViewOrganization(this ClaimsPrincipal user, int? organizationId)
        {
            if (user.IsSystemAdmin())
                return true;

            var userOrgId = user.GetOrganizationId();
            return userOrgId == organizationId;
        }

        /// <summary>
        /// Check if user can create in the given organization
        /// </summary>
        public static bool CanCreateInOrganization(this ClaimsPrincipal user, int? organizationId)
        {
            return user.CanManageOrganization(organizationId);
        }

        /// <summary>
        /// Check if user can edit/delete in the given organization
        /// </summary>
        public static bool CanEditInOrganization(this ClaimsPrincipal user, int? organizationId)
        {
            return user.CanManageOrganization(organizationId);
        }
    }
}
