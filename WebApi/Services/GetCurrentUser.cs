using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Domain.Interfaces;

namespace WebApi.Services
{
    public class GetCurrentUser : IUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetCurrentUser> _logger;

        public GetCurrentUser(IHttpContextAccessor httpContextAccessor, ILogger<GetCurrentUser> logger)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public string getCurrentUser()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.Claims?
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims.");
                return string.Empty;
            }

            return userId;
        }
        
        public bool IsInRole(string role)
        {
            if (string.IsNullOrEmpty(role))
                return false;
                
            var userRole = _httpContextAccessor.HttpContext?.User?.Claims?
                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                
            if (string.IsNullOrEmpty(userRole))
                return false;
                
            return string.Equals(userRole, role, StringComparison.OrdinalIgnoreCase);
        }
    }
}