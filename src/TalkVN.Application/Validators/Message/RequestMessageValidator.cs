using TalkVN.Application.Models.Dtos.Message;

using FluentValidation;

namespace TalkVN.Application.Validators.Message
{
    public class RequestMessageValidator : AbstractValidator<RequestSendMessageDto>
    {
        public RequestMessageValidator()
        {
            RuleFor(message => message.MessageText).NotEmpty().NotNull();
        }
    }
}
