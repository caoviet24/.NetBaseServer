using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class UnauthorizedAccessException : Exception
    {
        public UnauthorizedAccessException() : base("Not Unauthorized")
        {
            
        }
    }
}