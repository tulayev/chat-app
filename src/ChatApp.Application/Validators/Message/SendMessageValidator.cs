using ChatApp.Application.CQRS.Messages.Commands;
using FluentValidation;

namespace ChatApp.Application.Validators.Message
{
    public class SendMessageValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageValidator()
        {
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}
