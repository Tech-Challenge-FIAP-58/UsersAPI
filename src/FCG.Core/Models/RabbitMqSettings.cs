namespace FCG.Core.Models
{
	public class RabbitMqSettings
	{
		public required string Host { get; set; }
		public required string VirtualHost { get; set; }
		public required string UserName { get; set; }
		public required string Password { get; set; }
	}
}
