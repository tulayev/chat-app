using ChatApp.Application.DTOs.Message;
using ChatApp.Domain.Models;
using ChatApp.Tests.TestHelpers;
using Mapster;

namespace ChatApp.Tests.Mappings
{
    public class MessageMappingTests
    {
        private static Message BuildMessage() => new()
        {
            Id = 5,
            Content = "hello",
            SentAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            Sender = new AppUser { Id = 1, UserName = "alice", Email = "alice@example.com" }
        };

        [Fact]
        public void Adapt_MessageToMessageDto_MapsScalarFields()
        {
            var dto = BuildMessage().Adapt<MessageDto>(MapsterTestConfig.Instance);

            Assert.Equal(5, dto.Id);
            Assert.Equal("hello", dto.Content);
            Assert.Equal(new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc), dto.SentAt);
        }

        [Fact]
        public void Adapt_MessageToMessageDto_MapsNestedSenderToUserDto()
        {
            var dto = BuildMessage().Adapt<MessageDto>(MapsterTestConfig.Instance);

            Assert.Equal("alice", dto.Sender.Username);
            Assert.Equal("alice@example.com", dto.Sender.Email);
        }
    }
}
