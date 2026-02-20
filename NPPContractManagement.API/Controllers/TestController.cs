using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace NPPContractManagement.API.Controllers
{
    /// <summary>
    /// Test controller for health checks and authentication testing
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Health check endpoint to verify API is running
        /// </summary>
        /// <returns>API health status and system information</returns>
        /// <response code="200">API is healthy</response>
        [HttpGet("health")]
        [ProducesResponseType(200)]
        public ActionResult<object> HealthCheck()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
            });
        }

        /// <summary>
        /// Test endpoint that requires authentication
        /// </summary>
        /// <returns>User information and roles if authenticated</returns>
        /// <response code="200">Authentication successful</response>
        /// <response code="401">Unauthorized - valid JWT token required</response>
        [HttpGet("auth-test")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public ActionResult<object> AuthTest()
        {
            var user = User.Identity?.Name ?? "Unknown";
            var roles = User.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return Ok(new
            {
                message = "Authentication successful",
                user = user,
                roles = roles,
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Test endpoint that requires System Administrator role
        /// </summary>
        /// <returns>Success message if user has admin role</returns>
        /// <response code="200">Admin access successful</response>
        /// <response code="401">Unauthorized - valid JWT token required</response>
        /// <response code="403">Forbidden - System Administrator role required</response>
        [HttpGet("admin-test")]
        [Authorize(Roles = "System Administrator")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public ActionResult<object> AdminTest()
        {
            return Ok(new
            {
                message = "Admin access successful",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
