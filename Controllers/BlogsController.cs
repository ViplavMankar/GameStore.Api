using System;
using System.Security.Claims;
using GameStore.Api.DTOs;
using GameStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogsController : ControllerBase
{
    private readonly IBlogService _service;

    public BlogsController(IBlogService service, IHttpClientFactory httpClientFactory)
    {
        _service = service;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost("Create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] BlogCreateDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        // Console.WriteLine("userId: " + userId);
        var blog = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetAll), new { id = blog.Id }, blog);
    }

    [HttpPut("Edit/{id}")]
    [Authorize]
    public async Task<IActionResult> Edit([FromBody] BlogEditDto dto, Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        // Console.WriteLine("userId: " + userId);
        var blog = await _service.EditAsync(id, dto, userId);
        return CreatedAtAction(nameof(GetAll), new { id = blog.Id }, blog);
    }

    [HttpDelete("Delete/{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        // Console.WriteLine("userId: " + userId);
        var blog = await _service.DeleteAsync(id, userId);
        return NoContent();
    }

    [HttpGet("Mine")]
    [Authorize]
    public async Task<IActionResult> GetMyBlogs()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.GetMyBlogsAsync(userId));
    }

    [HttpGet("Mine/{id}")]
    [Authorize]
    public async Task<IActionResult> GetMySingleBlog(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var blog = await _service.GetMySingleBlogAsync(userId, id);

        if (blog == null)
            return NotFound(new { message = "Blog not found or you are not authorized to access it." });

        return Ok(blog);
    }
}
