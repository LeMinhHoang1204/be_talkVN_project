using TalkVN.Application.Models.Dtos.Message;
using FluentValidation;

namespace TalkVN.Application.Validators.Message
{
    public class RequestSearchMessageValidator : AbstractValidator<RequestSearchMessageDto>
    {
        public RequestSearchMessageValidator()
        {
            RuleFor(p => p.Text).NotNull().NotEmpty();
            RuleFor(p => p.ConversationId).NotNull().NotEmpty();
        }
    }
}
