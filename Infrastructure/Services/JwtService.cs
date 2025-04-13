using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entites;
using Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services
{
    public class JwtService : IJwtService
    {

        private readonly JwtConfiguration _jwtConfiguration;
        public JwtService(IOptions<JwtConfiguration> jwtConfiguration)
        {
            _jwtConfiguration = jwtConfiguration.Value;
        }
        public string generateAccessToken(User payload)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtConfiguration.AccessKey));
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, payload.Id.ToString()),
                new Claim(ClaimTypes.Role, payload.role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfiguration.AccessExpirationMinutes),
                Issuer = _jwtConfiguration.Issuer,
                Audience = _jwtConfiguration.Audience,
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.CreateToken(token);
            return tokenHandler.WriteToken(tokenString);
        }
        public string generateRefreshToken(User payload)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtConfiguration.RefreshKey));
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, payload.Id.ToString()),
                new Claim(ClaimTypes.Role, payload.role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(_jwtConfiguration.RefreshExpirationMinutes),
                Issuer = _jwtConfiguration.Issuer,
                Audience = _jwtConfiguration.Audience,
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.CreateToken(token);
            return tokenHandler.WriteToken(tokenString);
        }
    }
}