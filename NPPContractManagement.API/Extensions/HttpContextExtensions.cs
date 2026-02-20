using System.Security.Claims;

namespace NPPContractManagement.API.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Gets the audit principal in the format "FirstName LastName (email)" from JWT claims.
        /// Falls back to UserId if name/email claims are not available.
        /// </summary>
        public static string GetAuditPrincipal(this HttpContext httpContext)
        {
            var user = httpContext.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return "System";
            }

            var firstName = user.FindFirst("firstName")?.Value;
            var lastName = user.FindFirst("lastName")?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var userId = user.FindFirst(ClaimTypes.Name)?.Value; // UserId field

            // Build the audit principal string
            var name = $"{firstName} {lastName}".Trim();
            
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(email))
            {
                return $"{name} ({email})";
            }
            else if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }
            else if (!string.IsNullOrWhiteSpace(email))
            {
                return email;
            }
            else if (!string.IsNullOrWhiteSpace(userId))
            {
                return userId;
            }
            
            return "System";
        }
    }
}

