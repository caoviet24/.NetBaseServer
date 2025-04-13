using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entites
{
    public class User : AuditableEntity
    {
        public string username { get; set; } = null!;
        public string password { get; set; } = null!;
        public string role { get; set; } = null!;
    }
}