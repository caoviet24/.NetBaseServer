using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entites;
using Domain.Enums;
using Infrastructure.data.context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data.DbContext
{
    public class ApplicationDbContextInitialiser
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;

        public ApplicationDbContextInitialiser(
            ApplicationDbContext context,
            ILogger<ApplicationDbContextInitialiser> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                await _context.Database.EnsureCreatedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            // Default user
            var adminRoleString = Role.ADMIN.ToString();
            var adminExists = await _context.Users.AnyAsync(u => u.role == adminRoleString);
            if (adminExists)
            {
                _logger.LogInformation("Admin user already exists. Skipping seeding.");
                return;
            }
            

            if (!adminExists)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("123456");
                _logger.LogInformation($"Created admin user with password hash: {hashedPassword}");
                
                var admin = new User
                {
                    username = "admin",
                    password = hashedPassword,
                    role = adminRoleString,

                    createdBy = "system",
                    createdAt = DateTime.UtcNow,
                    isDeleted = false
                };

                _context.Users.Add(admin);
                
                await _context.SaveChangesAsync();
                _logger.LogInformation("Admin user seeded successfully.");
            }
        }
    }
}