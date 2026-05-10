using MediatR;
using Tawasol.Application.Common.Models;

namespace Tawasol.Application.Features.Cases.Commands.AddCaseAttachments;

public record FileModel(Stream Stream, string FileName, string ContentType);

public record AddCaseAttachmentsCommand(Guid CaseId, List<FileModel> Files) : IRequest<Result<bool>>;
