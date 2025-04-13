using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Common.Sercurity
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizeAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the role that is permitted to access the resource.
        /// </summary>
        public string? Role { get; set; } = string.Empty;
    }
}