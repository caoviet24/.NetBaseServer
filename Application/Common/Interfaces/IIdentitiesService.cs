using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.DTOs;

namespace Application.Common.Interfaces
{
    public interface IIdentitiesService
    {
        Task<UserDto> AuthMe(string token);
        Task<TokenDto> RefreshToken(string token);
    }
}