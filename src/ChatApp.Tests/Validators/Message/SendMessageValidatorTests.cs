using ChatApp.Application.CQRS.Messages.Commands;
using ChatApp.Application.Validators.Message;
using FluentValidation.TestHelper;

namespace ChatApp.Tests.Validators.Message
{
    public class SendMessageValidatorTests
    {
        private readonly SendMessageValidator _validator = new();

        [Fact]
        public void Should_HaveError_When_ContentIsEmpty()
        {
            var command = new SendMessageCommand { ChatId = 1, SenderId = 1, Content = "" };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Content);
        }

        [Fact]
        public void Should_HaveError_When_ContentIsNull()
        {
            var command = new SendMessageCommand { ChatId = 1, SenderId = 1, Content = null };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Content);
        }

        [Fact]
        public void Should_NotHaveError_When_ContentIsProvided()
        {
            var command = new SendMessageCommand { ChatId = 1, SenderId = 1, Content = "hello" };

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.Content);
        }
    }
}
