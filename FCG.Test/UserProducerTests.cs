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
            var endpointMock = new Mock<ISendEndpoint>();
            endpointMock.Setup(e => e.Send(It.IsAny<UserCreatedEvent>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask)
                        .Verifiable();

            busMock.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>()))
                   .ReturnsAsync(endpointMock.Object);

            var loggerMock = new Mock<ILogger<UserProducer>>();
            var producer = new UserProducer(loggerMock.Object, busMock.Object);

            await producer.PublishUserCreatedEvent(10, "a@b.com");

            busMock.Verify(b => b.GetSendEndpoint(new Uri("queue:notification-queue")), Times.Once);
            endpointMock.Verify(e => e.Send(
                It.Is<UserCreatedEvent>(m => m.Destinatario == "a@b.com" && m.Corpo.Contains("10")),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}