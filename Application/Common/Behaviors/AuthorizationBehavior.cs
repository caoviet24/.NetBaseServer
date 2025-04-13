using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Sercurity;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Common.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IUser _currentUser;
        
        public AuthorizationBehavior(IUser currentUser)
        {
            _currentUser = currentUser;
        }
        
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var authorizeAttributes = request.GetType()
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Cast<AuthorizeAttribute>()
                .ToList();

            // If no authorization attributes, allow the request
            if (!authorizeAttributes.Any())
            {
                return await next();
            }

            // Check if user is authenticated
            var userId = _currentUser.getCurrentUser();
            if (string.IsNullOrEmpty(userId))
            {
                throw new Domain.Exceptions.UnauthorizedAccessException();
            }

            // Check if user has required roles
            foreach (var attribute in authorizeAttributes)
            {
                if (!string.IsNullOrEmpty(attribute.Role))
                {
                    var hasRole = _currentUser.IsInRole(attribute.Role);
                    if (!hasRole)
                    {
                        throw new ForbiddenAccessException();
                    }
                }
            }

            // User is authorized, proceed with the request
            return await next();
        }
    }
}