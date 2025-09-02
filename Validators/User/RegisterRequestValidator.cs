using System;
using FluentValidation;
using SocialWebsite.DTOs.User;

namespace SocialWebsite.Validators.User;

public class RegisterRequestValidator :AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(request => request.UserName)
            .NotEmpty().WithMessage("User name must not empty")
            .MaximumLength(50).WithMessage("User name cannot exceed 50 characters");
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Email name must not empty")
            .EmailAddress().WithMessage("Ivalid email format");
        RuleFor(request => request.Password)
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number");
        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("First name must not empty")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");
        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("Last name must not empty")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");
        RuleFor(request => request.DateOfBirth)
            .LessThanOrEqualTo(DateTime.UtcNow.Date)
            .When(x => x.DateOfBirth.HasValue)
            .WithMessage("Birth date must be in the past");
        RuleFor(request => request.Gender)
            .IsInEnum()
            .WithMessage("Gender is not valid");
    }
}
