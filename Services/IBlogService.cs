using System;
using GameStore.Api.DTOs;

namespace GameStore.Api.Services;

public interface IBlogService
{
    Task<IEnumerable<BlogReadUserDto>> GetAllAsync();
    Task<BlogReadDto> CreateAsync(BlogCreateDto dto, Guid authorId);
    Task<BlogReadDto> EditAsync(Guid id, BlogEditDto dto, Guid authorId);
    Task<bool> DeleteAsync(Guid id, Guid authorId);

    Task<IEnumerable<BlogReadDto>> GetMyBlogsAsync(Guid userId);
    Task<BlogReadDto?> GetMySingleBlogAsync(Guid userId, Guid blogId);
}
