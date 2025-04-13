using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUser
    {
        public string getCurrentUser();
        
        /// <summary>
        /// Checks if the current user has the specified role.
        /// </summary>
        /// <param name="role">The role to check against.</param>
        /// <returns>True if the user has the role; otherwise, false.</returns>
        public bool IsInRole(string role);
    }
}