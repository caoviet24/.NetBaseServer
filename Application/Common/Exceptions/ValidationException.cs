using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(Dictionary<string, string[]> errors) : base("Validation error message") => Errors = errors;
        public Dictionary<string, string[]> Errors { get; }

    }
}