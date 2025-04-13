using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Common.DTOs
{
    public class TokenDto
    {
        public string accessToken { get; set; } = null!;
        public string refreshToken { get; set; } = null!;
    }
}