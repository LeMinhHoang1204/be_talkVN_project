using TalkVN.Application.Models.Dtos.User.Profile;
using TalkVN.Domain.Enums;

using FluentValidation;

namespace TalkVN.Application.Validators.User
{
    public class ProfileRequestValidator : AbstractValidator<ProfileRequestDto>
    {
        public ProfileRequestValidator()
        {
            RuleFor(profile => profile.Gender)
                .NotNull()
                .NotEmpty()
                .Must(gender => Enum.TryParse(typeof(Gender), gender, true, out _))
                .WithMessage("Invalid gender value. Allowed values are: Male, Female, Other.");
            RuleFor(profile => profile.Bio)
                .NotNull()
                .NotEmpty();
            RuleFor(profile => profile.DateOfBirth)
                .NotEmpty()
                .NotNull();
        }
    }
}
