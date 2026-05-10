namespace Tawasol.Application.Interfaces.Services;

public interface IFileService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default);
    void DeleteFile(string filePath);
}
