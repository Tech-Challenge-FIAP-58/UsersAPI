using MassTransit;
using Microsoft.Extensions.Logging;
using UserService.Core.Events;

namespace UserService.Application.Producer;

public class UserProducer(ILogger<UserProducer> logger, IBus bus)
{
    public async Task PublishUserCreatedEvent()
    {
        await bus.Publish(new UserCreatedEvent
        {
            Content = "Novo usuário criado!"
        });

        logger.LogInformation("UserCreatedEvent published to the queue.");
	}
}