using ChatApp.Application.Common.Behaviors;
using ChatApp.Application.CQRS.Login.Queries;
using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Helpers;
using ChatApp.Tests.Behaviors.TestOnly;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace ChatApp.Tests.Behaviors
{
    public class ValidationBehaviorTests
    {
        private static LoginUserQuery BuildQuery() => new(new LoginRequestDto("user", "password"));

        [Fact]
        public async Task Handle_NoValidatorsRegistered_CallsNext()
        {
            var behavior = new ValidationBehavior<LoginUserQuery, ApiResponse<string>>(
                Enumerable.Empty<IValidator<LoginUserQuery>>());
            var canned = ApiResponse<string>.Ok("x");
            var nextCalled = false;
            RequestHandlerDelegate<ApiResponse<string>> next = _ => { nextCalled = true; return Task.FromResult(canned); };

            var result = await behavior.Handle(BuildQuery(), next, CancellationToken.None);

            Assert.True(nextCalled);
            Assert.Same(canned, result);
        }

        [Fact]
        public async Task Handle_AllValidatorsPass_CallsNext()
        {
            var validatorMock = new Mock<IValidator<LoginUserQuery>>();
            validatorMock.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<LoginUserQuery>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var behavior = new ValidationBehavior<LoginUserQuery, ApiResponse<string>>([validatorMock.Object]);
            var canned = ApiResponse<string>.Ok("x");
            var nextCalled = false;
            RequestHandlerDelegate<ApiResponse<string>> next = _ => { nextCalled = true; return Task.FromResult(canned); };

            var result = await behavior.Handle(BuildQuery(), next, CancellationToken.None);

            Assert.True(nextCalled);
            Assert.Same(canned, result);
        }

        [Fact]
        public async Task Handle_ValidationFails_ResponseIsApiResponseOfT_ReturnsReflectiveFailWithJoinedMessages()
        {
            var validatorMock = new Mock<IValidator<LoginUserQuery>>();
            validatorMock.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<LoginUserQuery>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("UsernameOrEmail", "A required"),
                    new ValidationFailure("Password", "B required")
                }));

            var behavior = new ValidationBehavior<LoginUserQuery, ApiResponse<string>>([validatorMock.Object]);
            var nextCalled = false;
            RequestHandlerDelegate<ApiResponse<string>> next = _ => { nextCalled = true; return Task.FromResult(ApiResponse<string>.Ok("x")); };

            var result = await behavior.Handle(BuildQuery(), next, CancellationToken.None);

            Assert.False(nextCalled);
            Assert.False(result.Success);
            Assert.Equal("A required; B required", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_ValidationFails_ResponseIsNotApiResponseOfT_ThrowsValidationException()
        {
            var validatorMock = new Mock<IValidator<TestOnlyCommand>>();
            validatorMock.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestOnlyCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Value", "required") }));

            var behavior = new ValidationBehavior<TestOnlyCommand, TestOnlyResponse>([validatorMock.Object]);
            RequestHandlerDelegate<TestOnlyResponse> next = _ => Task.FromResult(new TestOnlyResponse());

            await Assert.ThrowsAsync<ValidationException>(() =>
                behavior.Handle(new TestOnlyCommand(), next, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_MultipleValidatorsRegistered_AggregatesFailuresFromAll()
        {
            var validator1 = new Mock<IValidator<LoginUserQuery>>();
            validator1.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<LoginUserQuery>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("UsernameOrEmail", "A required") }));
            var validator2 = new Mock<IValidator<LoginUserQuery>>();
            validator2.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<LoginUserQuery>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Password", "B required") }));

            var behavior = new ValidationBehavior<LoginUserQuery, ApiResponse<string>>([validator1.Object, validator2.Object]);
            RequestHandlerDelegate<ApiResponse<string>> next = _ => Task.FromResult(ApiResponse<string>.Ok("x"));

            var result = await behavior.Handle(BuildQuery(), next, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Contains("A required", result.ErrorMessage);
            Assert.Contains("B required", result.ErrorMessage);
        }
    }
}
