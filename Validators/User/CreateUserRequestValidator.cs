using System;
using FluentValidation;
using SocialWebsite.DTOs.User;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.UserName)
            .NotEmpty().WithMessage("User name must not empty")
            .MaximumLength(50).WithMessage("User name cannot exceed 50 characters");
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Email name must not empty")
            .EmailAddress().WithMessage("Ivalid email format");
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
