namespace Tawasol.Domain.Entities;

public class CaseAttachment
{
    public Guid Id { get; private set; }
    public Guid CaseId { get; private set; }
    public string FileName { get; private set; }
    public string FilePath { get; private set; }
    public string FileType { get; private set; }
    public DateTime UploadDate { get; private set; }

    private CaseAttachment() { }

    public CaseAttachment(Guid caseId, string fileName, string filePath, string fileType)
    {
        // We leave Id as default (Guid.Empty) so EF Core knows it's an INSERT
        CaseId = caseId;
        FileName = fileName;
        FilePath = filePath;
        FileType = fileType;
        UploadDate = DateTime.UtcNow;
    }
}
