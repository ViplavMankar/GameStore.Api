using System;
using GameStore.Api.DTOs;

namespace GameStore.Api.Services;

public interface IBlogService
{
    Task<IEnumerable<BlogReadUserDto>> GetAllAsync();
    Task<BlogReadUserDto?> GetByIdWithUsernameAsync(Guid id);
    Task<BlogReadDto> CreateAsync(BlogCreateDto dto, Guid authorId);
    Task<BlogReadDto> EditAsync(Guid id, BlogEditDto dto, Guid authorId);
    Task<bool> DeleteAsync(Guid id, Guid authorId);

    Task<IEnumerable<BlogReadUserDto>> GetMyBlogsAsync(Guid userId);
    Task<BlogReadDto?> GetMySingleBlogAsync(Guid userId, Guid blogId);
}
