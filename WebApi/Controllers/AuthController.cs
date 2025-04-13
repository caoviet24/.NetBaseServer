using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Identites.Commands.SignIn;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ISender sender, ILogger<AuthController> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] SignInCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(new
            {
                message = "Đăng nhập thành công",
                success = true,
                accessToken = result.accessToken,
                refreshToken = result.refreshToken
            });
        }

        // [HttpPost("register")]
        // public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        // {
        //     await _sender.Send(command);
        //     return StatusCode(201, new { 
        //         message = "Đăng ký thành công",
        //         success = true
        //     });
        // }

        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = HttpContext.Request.Headers["refresh_token"].ToString();
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "No refresh token provided", success = false });
            }

            try
            {
                var authService = HttpContext.RequestServices.GetRequiredService<IIdentitiesService>();
                var result = await authService.RefreshToken(refreshToken);

                return Ok(new
                {
                    accessToken = result.accessToken,
                    refreshToken = result.refreshToken
                });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message, success = false });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var authService = HttpContext.RequestServices.GetRequiredService<IIdentitiesService>();
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var user = await authService.AuthMe(token);
            return Ok(user);
        }
    }
}