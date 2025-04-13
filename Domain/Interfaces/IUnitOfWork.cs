using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    /// <summary>
    /// Unit of Work pattern interface to coordinate the work of multiple repositories
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// Repository for managing entities
        IUserRepository Users { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}