using TalkVN.Application.Models.Dtos.Post.CreatePost;
using TalkVN.Domain.Enums;

using FluentValidation;

namespace TalkVN.Application.Validators.Post
{
    public class CreatePostMediaRequestValidator : AbstractValidator<CreatePostMediaRequestDto>
    {
        public CreatePostMediaRequestValidator()
        {
            RuleFor(p => p.MediaType)
                .Must(value => Enum.TryParse(typeof(MediaType), value?.ToString(), out _))
                .WithMessage("Media type must be a valid enum value");
        }
    }
}
