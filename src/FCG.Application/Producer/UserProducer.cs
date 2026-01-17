using MassTransit;
using Microsoft.Extensions.Logging;
using FCG.Core.Messages.Integration;

namespace FCG.Application.Producer;

public class UserProducer(ILogger<UserProducer> logger, IBus bus)
{
    public async Task PublishUserCreatedEvent(int userId, string email)
    {
        await bus.Publish(new UserCreatedEvent
        {
            UserId = userId,
            Email = email
        });

        logger.LogInformation("UserCreatedEvent published to the queue.");
	}
}