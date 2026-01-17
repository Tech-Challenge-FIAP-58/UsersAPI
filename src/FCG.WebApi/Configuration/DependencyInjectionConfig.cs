using FCG.Core.Models;
using MassTransit;

namespace FCG.WebApi.Configuration
{
	public static class DependencyInjectionConfig
	{
		public static void RegisterConfigurations(this WebApplicationBuilder builder)
		{
			var rabbitMqConfigSection = builder.Configuration.GetSection("RabbitMQ");
			builder.Services.Configure<RabbitMqSettings>(rabbitMqConfigSection);
		}

		public static void RegisterMassTransit(this WebApplicationBuilder builder)
		{
			var settings = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>()
				?? throw new NullReferenceException("RabbitMQ configuration section is missing or invalid.");

			builder.Services.AddMassTransit(x =>
			{
                x.SetKebabCaseEndpointNameFormatter();
				x.SetInMemorySagaRepositoryProvider();

				x.UsingRabbitMq((context, cfg) =>
				{
					cfg.Host(settings.Host, settings.VirtualHost, h =>
					{
						h.Username(settings.UserName);
						h.Password(settings.Password);
					});
					cfg.ConfigureEndpoints(context);
				});
			});
		}
	}
}
