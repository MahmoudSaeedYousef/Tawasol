using FluentValidation;

namespace Tawasol.Application.Features.Cases.Commands.RejectCase;

public class RejectCaseCommandValidator : AbstractValidator<RejectCaseCommand>
{
    public RejectCaseCommandValidator()
    {
        // RuleFor(x => x.Reason)
        //     .NotEmpty().WithMessage("A rejection reason is required.")
        //     .MinimumLength(10).WithMessage("Please provide a more detailed rejection reason.");
    }
}
