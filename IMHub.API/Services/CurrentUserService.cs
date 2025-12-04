using System.Security.Claims;
using IMHub.ApplicationLayer.Common.Interfaces;

namespace IMHub.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId 
        {
            get
            {
                var id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) 
                      ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");
                
                if (string.IsNullOrEmpty(id) || !int.TryParse(id, out int userId))
                {
                    return null;
                }
                
                return userId;
            }
        }

        public int? OrganizationId 
        {
            get
            {
                // Check both "OrganizationId" (from JWT) and "tenantId" (alternative)
                var orgId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("OrganizationId")
                         ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("tenantId");
                
                if (string.IsNullOrEmpty(orgId) || !int.TryParse(orgId, out int organizationId))
                {
                    return null;
                }
                
                return organizationId;
            }
        }

        public string? Role 
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role)
                    ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("role");
            }
        }
    }
}

