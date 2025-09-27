using System;
using FluentValidation;
using SocialWebsite.DTOs.Post;

namespace SocialWebsite.Validators.Post;

public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(request => request.Content)
            .NotEmpty().WithMessage("Nội dung bài viết không được để trống")
            .MaximumLength(5000).WithMessage("Nội dung bài viết không được vượt quá 5000 ký tự");

        RuleFor(request => request.Privacy)
            .IsInEnum().WithMessage("Quyền riêng tư không hợp lệ");
        
    }
}
