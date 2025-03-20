using FluentValidation;

namespace TaskManagement.Application.Tasks.Commands.RejectTask
{
    public class RejectTaskCommandValidator : AbstractValidator<RejectTaskCommand>
    {
        public RejectTaskCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");
        }
    }
} 