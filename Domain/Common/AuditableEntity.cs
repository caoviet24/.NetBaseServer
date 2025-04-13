using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Common
{
    public abstract class AuditableEntity : BaseEnity
    {
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public string createdBy { get; set; } = null!;
        public DateTime? updatedAt { get; set; }
        public string? updatedBy { get; set; }
        public bool? isDeleted { get; set; }
        public string? deletedBy { get; set; }
        public DateTime? deletedAt { get; set; }
    }
}