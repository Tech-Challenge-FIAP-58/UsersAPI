using MassTransit;
using Microsoft.Extensions.Logging;
using FCG.Core.Messages.Integration;

namespace FCG.Application.Producer;

// IPublishEndpoint é a interface real com o método Publish() — pode ser mockada com Moq.
// IBus herda de IPublishEndpoint, então a injeção em produção continua funcionando
// passando o IBus registrado no container. Em testes, basta mockar IPublishEndpoint diretamente.
public class UserProducer(ILogger<UserProducer> logger, IPublishEndpoint publishEndpoint)
{
    public async Task PublishUserCreatedEvent(int userId, string email)
    {
        logger.LogInformation("Publishing UserCreatedEvent for user ID: {UserId}.", userId);

        await publishEndpoint.Publish(new UserCreatedEvent
        {
            Destinatario = email,
            Assunto = "Bem-vindo ao FCG!",
            Corpo = $"Olá! Seu usuário com ID {userId} foi criado com sucesso."
        });

        logger.LogInformation("UserCreatedEvent published for user ID: {UserId}.", userId);
    }
}
