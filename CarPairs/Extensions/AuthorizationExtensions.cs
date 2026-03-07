using System.Security.Claims;

namespace CarPairs.Web.Extensions
{
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Get the user's organization ID from claims
        /// </summary>
        public static int? GetOrganizationId(this ClaimsPrincipal user)
        {
            var orgClaim = user.FindFirst("OrganizationId");
            if (orgClaim != null && int.TryParse(orgClaim.Value, out var orgId))
                return orgId;

            return null;
        }

        /// <summary>
        /// Get the user's role from claims
        /// </summary>
        public static string? GetUserRole(this ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value;
        }

        /// <summary>
        /// Check if user is in the organization
        /// </summary>
        public static bool IsInOrganization(this ClaimsPrincipal user, int? organizationId)
        {
            if (organizationId == null)
                return false;

            var userOrgId = user.GetOrganizationId();
            return userOrgId == organizationId;
        }

        /// <summary>
        /// Check if user can manage the organization (Admin or Manager role)
        /// </summary>
        public static bool CanManageOrganization(this ClaimsPrincipal user, int? organizationId)
        {
            if (user.IsSystemAdmin())
                return true;

            if (!user.IsInOrganization(organizationId))
                return false;

            var role = user.GetUserRole();
            return role == "Admin" || role == "Manager";
        }

        /// <summary>
        /// Check if user is system admin
        /// </summary>
        public static bool IsSystemAdmin(this ClaimsPrincipal user)
        {
            var role = user.GetUserRole();
            return role == "Admin" && user.GetOrganizationId() == null;
        }
    }
}
