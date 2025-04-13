using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    public class JwtConfiguration
    {
        public string AccessKey { get; set; } = null!;
        public string RefreshKey { get; set; } = null!;
        public int AccessExpirationMinutes { get; set; }
        public int RefreshExpirationMinutes { get; set; }
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int ExpirationMinutes { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public bool RequireExpirationTime { get; set; }
        public bool ValidateLifetime { get; set; }
    }
}