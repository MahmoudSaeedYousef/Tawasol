using FluentValidation;

namespace Tawasol.Application.Features.Cases.Commands.CreateCase;

public class CreateCaseCommandValidator : AbstractValidator<CreateCaseCommand>
{
    public CreateCaseCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(v => v.TargetAmount)
            .GreaterThan(0).WithMessage("Target amount must be greater than 0.");

        RuleFor(v => v.CaseType)
            .NotEmpty().WithMessage("Case type is required.");

        When(v => v.CaseType == "Medical", () =>
        {
            RuleFor(v => v.ExtraDetails)
                .Must(d => d != null && d.ContainsKey("HospitalName"))
                .WithMessage("Medical cases must provide a HospitalName in ExtraDetails.");
        });

        When(v => v.CaseType == "Debt", () =>
        {
            RuleFor(v => v.ExtraDetails)
                .Must(d => d != null && d.ContainsKey("CreditorName"))
                .WithMessage("Debt cases must provide a CreditorName in ExtraDetails.");
        });
    }
}
