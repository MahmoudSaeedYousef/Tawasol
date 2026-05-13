using System.Text.Json;
using FluentValidation;
using Tawasol.Domain.Enums;

namespace Tawasol.Application.Features.Cases.Commands.CreateCase;

public class CreateCaseCommandValidator : AbstractValidator<CreateCaseCommand>
{
    public CreateCaseCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("العنوان مطلوب")
            .MaximumLength(200).WithMessage("العنوان لا يمكن أن يتجاوز 200 حرف");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("الوصف مطلوب");

        RuleFor(v => v.CaseType)
            .NotEmpty().WithMessage("نوع الحالة مطلوب");

        RuleFor(v => v.TargetAmount)
            .GreaterThan(0).When(v => v.CaseType == CaseItemType.Monetary /*|| v.CaseType == "Debt"*/)
            .WithMessage("المبلغ المستهدف يجب أن يكون أكبر من صفر.");

        // تحقق من صحة الـ JSON
        RuleFor(v => v.ExtraDetailsJson)
            .Must(BeValidJson).WithMessage("صيغة البيانات الإضافية غير صحيحة.");

        // // التحقق بناءً على النوع (استخدام الـ Property المـفكوك مباشرة)
        When(v => v.CaseType == CaseItemType.HospitalityType, () =>
        {
            RuleFor(v => v.ExtraDetails)
                .Must(d => d.ContainsKey("HospitalName") && !string.IsNullOrWhiteSpace(d["HospitalName"]))
                .WithMessage("الحالات الطبية يجب أن تشمل اسم المستشفى.");
        });

        When(v => v.CaseType == CaseItemType.DebtType, () =>
        {
            RuleFor(v => v.ExtraDetails)
                .Must(d => d.ContainsKey("CreditorName") && !string.IsNullOrWhiteSpace(d["CreditorName"]))
                .WithMessage("حالات الديون يجب أن تشمل اسم الدائن.");
        });
    }

    private bool BeValidJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return true;
        try
        {
            using var doc = JsonDocument.Parse(json);
            return true;
        }
        catch { return false; }
    }
}