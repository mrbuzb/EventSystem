using EventSystem.Application.Dtos;
using FluentValidation;

namespace ContactManagement.Application.Validators;

public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores.");

        RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
        .MaximumLength(128).WithMessage("Password must not exceed 128 characters.")
        .Matches(@"[A-Z]+").WithMessage("Password must contain at least one uppercase letter.")
        .Matches(@"[a-z]+").WithMessage("Password must contain at least one lowercase letter.")
        .Matches(@"[0-9]+").WithMessage("Password must contain at least one number.");
    }
}
