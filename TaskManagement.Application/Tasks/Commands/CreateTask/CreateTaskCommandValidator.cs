using FluentValidation;

namespace TaskManagement.Application.Tasks.Commands.CreateTask
{
    public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.CreatedById)
                .NotEmpty().WithMessage("CreatedById is required");

            RuleFor(x => x.AssignedToId)
                .NotEmpty().WithMessage("AssignedToId is required");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("DepartmentId is required");
        }
    }
} 