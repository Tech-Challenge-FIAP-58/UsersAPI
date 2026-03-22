using MassTransit;
using Microsoft.Extensions.Logging;
using FCG.Core.Messages.Integration;

namespace FCG.Application.Producer;

public class UserProducer(ILogger<UserProducer> logger, IBus bus)
{
    public async Task PublishUserCreatedEvent(int userId, string email)
    {
        var endpoint = await bus.GetSendEndpoint(new Uri("queue:notification-queue"));
        logger.LogInformation("Publishing UserCreatedEvent for user ID: {UserId} to notification-queue.", userId);
        await endpoint.Send(new UserCreatedEvent
        {
            Destinatario = email,
            Assunto = "Bem-vindo ao FCG!",
            Corpo = $"Olá! Seu usuário com ID {userId} foi criado com sucesso."
        });
        logger.LogInformation("UserCreatedEvent published for user ID: {UserId} to notification-queue.", userId);

        logger.LogInformation("UserCreatedEvent published to the notification-queue.");
	}
}
