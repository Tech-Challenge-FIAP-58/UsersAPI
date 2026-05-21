using MassTransit;
using Microsoft.Extensions.Logging;
using FCG.Core.Messages.Integration;
using FCG.Infra.Repository;
using FCG.Core.Models;

namespace FCG.Application.Producer;

// IPublishEndpoint é a interface real com o método Publish() — pode ser mockada com Moq.
// IBus herda de IPublishEndpoint, então a injeção em produção continua funcionando
// passando o IBus registrado no container. Em testes, basta mockar IPublishEndpoint diretamente.
public class UserProducer(ILogger<UserProducer> logger, IPublishEndpoint publishEndpoint, IEventLogRepository eventLogRepository)
{
    public async Task PublishUserCreatedEvent(int userId, string email)
    {
        logger.LogInformation("Publishing UserCreatedEvent for user ID: {UserId}.", userId);

        var subject = "Bem-vindo ao FCG!";
        var body = $"Olá! Seu usuário com ID {userId} foi criado com sucesso.";

		await publishEndpoint.Publish(new UserCreatedEvent
        {
            Destinatario = email,
            Assunto = subject,
            Corpo = body
		});

        logger.LogInformation("UserCreatedEvent published for user ID: {UserId}.", userId);
        await eventLogRepository.InsertUserProducerEventLog(new UserProducerEventLog
		{
			UserId = userId,
			Email = email,
			Subject = subject,
			Body = body,
            DtCpu = DateTime.Now
		});
	}
}
