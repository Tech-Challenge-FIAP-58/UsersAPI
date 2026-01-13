using MassTransit;
using Microsoft.Extensions.Logging;
using UserService.Core.Events;

namespace UserService.Application.Producer;

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