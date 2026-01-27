using FluentValidation;
using TaskList.Application.DTOs.Tasks;
using TaskList.Domain.Enums;

namespace TaskList.Application.Validators;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(1, 200).WithMessage("Title must be between 1 and 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.")
            .When(x => x.DueDate.HasValue);

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority must be a valid value: 1 (Low), 2 (Medium), or 3 (High).")
            .Must(BeValidPriority).WithMessage("Priority must be Low (1), Medium (2), or High (3).");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Category must be a valid value: 1 (Work), 2 (Personal), or 3 (Shopping).")
            .Must(BeValidCategory).WithMessage("Category must be Work (1), Personal (2), or Shopping (3).");
    }

    private bool BeValidPriority(PriorityLevel priority)
    {
        return priority is PriorityLevel.Low or PriorityLevel.Medium or PriorityLevel.High;
    }

    private bool BeValidCategory(CategoryType category)
    {
        return category is CategoryType.Work or CategoryType.Personal or CategoryType.Shopping;
    }
}
