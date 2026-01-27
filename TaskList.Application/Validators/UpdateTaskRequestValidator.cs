using FluentValidation;
using TaskList.Application.DTOs.Tasks;
using TaskList.Domain.Enums;

namespace TaskList.Application.Validators;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .Length(1, 200).WithMessage("Title must be between 1 and 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.")
            .When(x => x.DueDate.HasValue);

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority must be a valid value: 1 (Low), 2 (Medium), or 3 (High).")
            .Must(BeValidPriority).WithMessage("Priority must be Low (1), Medium (2), or High (3).")
            .When(x => x.Priority.HasValue);

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Category must be a valid value: 1 (Work), 2 (Personal), or 3 (Shopping).")
            .Must(BeValidCategory).WithMessage("Category must be Work (1), Personal (2), or Shopping (3).")
            .When(x => x.Category.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid value: 1 (Todo), 2 (InProgress), 3 (Completed), or 4 (Cancelled).")
            .Must(BeValidStatus).WithMessage("Status must be Todo (1), InProgress (2), Completed (3), or Cancelled (4).")
            .When(x => x.Status.HasValue);
    }

    private bool BeValidPriority(PriorityLevel? priority)
    {
        if (!priority.HasValue) return true;
        return priority.Value is PriorityLevel.Low or PriorityLevel.Medium or PriorityLevel.High;
    }

    private bool BeValidCategory(CategoryType? category)
    {
        if (!category.HasValue) return true;
        return category.Value is CategoryType.Work or CategoryType.Personal or CategoryType.Shopping;
    }

    private bool BeValidStatus(TaskItemStatus? status)
    {
        if (!status.HasValue) return true;
        return status.Value is TaskItemStatus.Todo or TaskItemStatus.InProgress or TaskItemStatus.Completed or TaskItemStatus.Cancelled;
    }
}
