
using FluentValidation;
using UsersApi.DTOs;

namespace UsersApi.Validators;

public class RegisterValidator : AbstractValidator<RegisterDTO>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required").
        MinimumLength(2).WithMessage("Name must be at least 2 characters");

        RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required")
        .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
        .WithMessage("Invalid email format");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required").
        MinimumLength(2).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.Age).GreaterThan(0).WithMessage("Age must be greater than 0").
        LessThan(120).WithMessage("Age must be greater tahn 120");

    }
}