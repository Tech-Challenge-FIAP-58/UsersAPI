namespace FCG.Core.Messages.Integration
{
	public class UserCreatedEvent
	{
		public required string Destinatario { get; set; }
		public required string Assunto { get; set; }
		public required string Corpo { get; set; }
	}
}
