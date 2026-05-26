using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Cases;
using Tawasol.Domain.Enums;

namespace Tawasol.Application.Features.Cases.Commands.CreateCase;

public class CreateCaseCommand : IRequest<Result<CaseResponseDto>>
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal TargetAmount { get; set; } // للحالات المادية
    public CaseItemType CaseType { get; set; } = default!;

    public string? ExtraDetailsJson { get; set; }
    public string? CaseItemsJson { get; set; }

    public List<IFormFile>? Attachments { get; set; }
    public Guid CreatedBy { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
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

// 🚀 التعديل الإستراتيجي 1: تطابق الاسم والنوع بالملي مع الفلاتر والداتابيز
public record CaseItemRequest(string Name, int TargetAmount, decimal EstimatedCost);