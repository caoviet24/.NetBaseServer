using Application.Common.DTOs;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RedisController : ControllerBase
{
    private readonly IRedisService _redisService;
    public RedisController(IRedisService redisService)
    {
        _redisService = redisService;
    }

    [HttpGet("string/{key}")]
    public async Task<IActionResult> GetString(string key)
    {
        var value = await _redisService.GetStringAsync<string>(key);
        if (value == null)
        {
            return NotFound(Response<object>.Fail($"Key {key} not found"));
        }

        return Ok(Response<string>.Success(value));
    }

    [HttpPost("string")]
    public async Task<IActionResult> SetString([FromQuery] string key, [FromBody] string value)
    {
        // Set with 1 hour expiry
        var success = await _redisService.SetStringAsync(key, value, TimeSpan.FromHours(1));

        if (success)
        {
            return Ok(Response<object>.Success("", "Value stored successfully"));
        }

        return BadRequest(Response<object>.Fail("Failed to store value"));
    }

    [HttpGet("health")]
    public async Task<IActionResult> CheckHealth()
    {
        try
        {
            var ping = await _redisService.GetStringAsync<string>("health-check");

            // If no health check exists, set one
            if (ping == null)
            {
                await _redisService.SetStringAsync("health-check", "ok", TimeSpan.FromMinutes(5));
                ping = "ok";
            }

            var healthData = new
            {
                Status = "Connected",
                LastPing = DateTime.Now
            };

            return Ok(Response<object>.Success(healthData, "Redis connection healthy"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, Response<string>.Fail($"Redis connection error: {ex.Message}"));
        }
    }
}