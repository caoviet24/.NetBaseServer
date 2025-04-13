using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entites;
namespace Application.Common.Interfaces
{
    public interface IJwtService
    {
        string generateAccessToken(User payload);
        string generateRefreshToken(User payload);
    }
}