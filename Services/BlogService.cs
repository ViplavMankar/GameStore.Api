using System;
using GameStore.Api.Data;
using GameStore.Api.DTOs;
using GameStore.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Services;

public class BlogService : IBlogService
{
    private readonly GameStoreDbContext _context;
    private readonly HttpClient _authClient;

    public BlogService(GameStoreDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _authClient = httpClientFactory.CreateClient("AuthService");
    }

    public async Task<IEnumerable<BlogReadUserDto>> GetAllAsync()
    {
        var blogs = await _context.Blogs
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        var result = new List<BlogReadUserDto>();

        foreach (var blog in blogs)
        {
            var username = await ResolveUsernameAsync(blog.AuthorUserId);

            result.Add(new BlogReadUserDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Content = blog.Content,
                AuthorUserId = blog.AuthorUserId,
                AuthorUsername = username,
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt
            });
        }
        return result;
    }

    public async Task<BlogReadDto> CreateAsync(BlogCreateDto dto, Guid authorId)
    {
        var blog = new Blog
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Content = dto.Content,
            AuthorUserId = authorId,
        };
        _context.Blogs.Add(blog);
        await _context.SaveChangesAsync();

        return new BlogReadDto
        {
            Id = blog.Id,
            Title = blog.Title,
            Content = blog.Content,
            AuthorUserId = blog.AuthorUserId,
            CreatedAt = blog.CreatedAt,
            UpdatedAt = blog.UpdatedAt
        };
    }

    public async Task<BlogReadDto> EditAsync(Guid id, BlogEditDto dto, Guid authorId)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog == null)
        {
            throw new KeyNotFoundException("Blog Not Found");
        }
        if (blog.AuthorUserId != authorId)
        {
            throw new UnauthorizedAccessException("You are not allowed to edit this blog!");
        }
        blog.Title = dto.Title ?? blog.Title;
        blog.Content = dto.Content ?? blog.Content;

        blog.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new BlogReadDto
        {
            Id = blog.Id,
            Title = blog.Title,
            Content = blog.Content,
            AuthorUserId = blog.AuthorUserId,
            CreatedAt = blog.CreatedAt,
            UpdatedAt = blog.UpdatedAt,
        };
    }

    public async Task<bool> DeleteAsync(Guid id, Guid authorId)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog == null)
        {
            return false;
        }
        if (blog.AuthorUserId != authorId)
        {
            throw new UnauthorizedAccessException("You are not allowed to delete this blog!");
        }
        _context.Blogs.Remove(blog);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<BlogReadDto>> GetMyBlogsAsync(Guid userId)
    {
        return await _context.Blogs
                            .Where(b => b.AuthorUserId == userId)
                            .Select(b => new BlogReadDto
                            {
                                Id = b.Id,
                                Title = b.Title,
                                Content = b.Content,
                                AuthorUserId = b.AuthorUserId,
                                CreatedAt = b.CreatedAt
                            }).ToListAsync();

    }

    public async Task<BlogReadDto?> GetMySingleBlogAsync(Guid userId, Guid blogId)
    {
        return await _context.Blogs
                            .Where(b => b.AuthorUserId == userId && b.Id == blogId)
                            .Select(b => new BlogReadDto
                            {
                                Id = b.Id,
                                Title = b.Title,
                                Content = b.Content,
                                AuthorUserId = b.AuthorUserId,
                                CreatedAt = b.CreatedAt
                            }).FirstOrDefaultAsync();
    }

    private async Task<string?> ResolveUsernameAsync(Guid? userId)
    {
        if (userId == null)
        {
            return "Anonymous";
        }
        try
        {
            var response = await _authClient.GetAsync($"api/users/{userId.ToString()}");
            if (!response.IsSuccessStatusCode)
                return "Unknown";

            var user = await response.Content.ReadFromJsonAsync<UserReadDto>();
            return user?.Username ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }
    private class UserReadDto
    {
        public string Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
