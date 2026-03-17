using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using FCG.Application.Producer;
using FCG.Core.Messages.Integration;
using Xunit;

namespace FCG.Test
{
    public class UserProducerTests
    {
        [Fact]
        public async Task PublishUserCreatedEvent_CallsBusPublish()
        {
            var busMock = new Mock<IBus>();
            busMock.Setup(b => b.Publish(It.IsAny<UserCreatedEvent>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask)
                   .Verifiable();

            var loggerMock = new Mock<ILogger<UserProducer>>();
            var producer = new UserProducer(loggerMock.Object, busMock.Object);

            await producer.PublishUserCreatedEvent(10, "a@b.com");

            busMock.Verify(b => b.Publish(It.Is<UserCreatedEvent>(e => e.UserId == 10 && e.Email == "a@b.com"), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}