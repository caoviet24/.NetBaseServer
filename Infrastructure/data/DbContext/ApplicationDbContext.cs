using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.data.context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// Configures the entity models and their relationships
        /// </summary>
        /// <param name="modelBuilder">The model builder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            // Enable filtering for soft delete
            // foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            // {
            //     if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
            //     {
            //         var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
            //         var property = System.Linq.Expressions.Expression.Property(parameter, "isDeleted");
            //         var nullConstant = System.Linq.Expressions.Expression.Constant(null, typeof(bool?));
            //         var falseConstant = System.Linq.Expressions.Expression.Constant(false, typeof(bool?));

            //         var nullCheck = System.Linq.Expressions.Expression.Equal(property, nullConstant);
            //         var falseCheck = System.Linq.Expressions.Expression.Equal(property, falseConstant);
            //         var orExpression = System.Linq.Expressions.Expression.OrElse(nullCheck, falseCheck);

            //         var lambda = System.Linq.Expressions.Expression.Lambda(orExpression, parameter);
            //         entityType.SetQueryFilter(lambda);
            //     }
            // }

        }
    }
}