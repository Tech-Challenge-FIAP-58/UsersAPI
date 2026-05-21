using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using FCG.Application.Producer;
using FCG.Core.Messages.Integration;
using FCG.Infra.Repository;

namespace FCG.Test
{
    public class UserProducerTests
    {
        [Fact]
        public async Task PublishUserCreatedEvent_CallsBusPublish()
        {
            var eventLogRepoMock = new Mock<IEventLogRepository>();
			var publishMock = new Mock<IPublishEndpoint>();
            publishMock
                .Setup(p => p.Publish(It.IsAny<UserCreatedEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var loggerMock = new Mock<ILogger<UserProducer>>();
            var producer = new UserProducer(loggerMock.Object, publishMock.Object, eventLogRepoMock.Object);

            await producer.PublishUserCreatedEvent(10, "a@b.com");

            publishMock.Verify(
                p => p.Publish(
                    It.Is<UserCreatedEvent>(e => e.Corpo.Contains("10") && e.Destinatario == "a@b.com"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
