using EventSystem.Application.Dtos;
using FluentValidation;

namespace EventSystem.Application.Validators;

public class EventUpdateDtoValidator : AbstractValidator<EventUpdateDto>
{
    public EventUpdateDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Date).GreaterThan(DateTime.Now);
        RuleFor(x => x.Capasity).GreaterThan(0);
    }
}