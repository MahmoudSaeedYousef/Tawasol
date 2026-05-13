using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Cases;
using Tawasol.Domain.Enums;

namespace Tawasol.Application.Features.Cases.Commands.CreateCase;

public class CreateCaseCommand : IRequest<Result<CaseResponseDto>>
{
    // الحقول الأساسية
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal TargetAmount { get; set; }
    public CaseItemType CaseType { get; set; } = default!;

    // الحقول الاختيارية (JSON Strings)
    public string? ExtraDetailsJson { get; set; }
    public string? CaseItemsJson { get; set; }

    // المرفقات (بقت اختياري 100% دلوقتي)
    public List<IFormFile>? Attachments { get; set; }

    // حقل المحقن يدوياً
    public Guid CreatedBy { get; set; }

    // --- اللوجيك بتاع الـ Deserialization زي ما هو ---
    private Dictionary<string, string>? _extraDetails;
    public Dictionary<string, string> ExtraDetails => _extraDetails ??= 
        string.IsNullOrWhiteSpace(ExtraDetailsJson) 
            ? new() 
            : JsonSerializer.Deserialize<Dictionary<string, string>>(ExtraDetailsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

    private List<CaseItemRequest>? _caseItems;
    public List<CaseItemRequest> CaseItems => _caseItems ??= 
        string.IsNullOrWhiteSpace(CaseItemsJson) 
            ? new() 
            : JsonSerializer.Deserialize<List<CaseItemRequest>>(CaseItemsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
}

public record CaseItemRequest(string Name, int Quantity, decimal EstimatedCost);