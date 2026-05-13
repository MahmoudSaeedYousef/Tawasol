using Microsoft.AspNetCore.Http;

namespace Tawasol.Application.Interfaces.Services;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file, string subFolder);
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default);

    void DeleteFile(string filePath);
}
