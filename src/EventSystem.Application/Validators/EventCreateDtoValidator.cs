using EventSystem.Application.Dtos;
using FluentValidation;

namespace EventSystem.Application.Validators;

public class EventCreateDtoValidator : AbstractValidator<EventCreateDto>
{
    public EventCreateDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.Date).GreaterThan(DateTime.Now).WithMessage("Date must be in the future.");
        RuleFor(x => x.Capasity).GreaterThan(0).WithMessage("Capacity must be greater than 0.");
        RuleFor(x => x.Location).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
