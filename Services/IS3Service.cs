using System;

namespace GameStore.Api.Services;

public interface IS3Service
{
    Task<string> UploadFileAsync(Stream fileStream, string key, string contentType);
    Task<string> UploadZipAsync(Stream zipStream, string gameName);
    Task DeleteFolderAsync(string folderName);
}
