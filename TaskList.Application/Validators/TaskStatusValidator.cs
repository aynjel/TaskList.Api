using FluentValidation;
using TaskList.Domain.Enums;

namespace TaskList.Application.Validators;

public class TaskStatusValidator : AbstractValidator<TaskItemStatus>
{
    public TaskStatusValidator()
    {
        RuleFor(status => status)
            .IsInEnum().WithMessage("Status must be a valid value: 1 (Todo), 2 (InProgress), 3 (Completed), or 4 (Cancelled).")
            .Must(BeValidStatus).WithMessage("Status must be Todo (1), InProgress (2), Completed (3), or Cancelled (4).");
    }

    private bool BeValidStatus(TaskItemStatus status)
    {
        return status is TaskItemStatus.Todo or TaskItemStatus.InProgress or TaskItemStatus.Completed or TaskItemStatus.Cancelled;
    }
}
