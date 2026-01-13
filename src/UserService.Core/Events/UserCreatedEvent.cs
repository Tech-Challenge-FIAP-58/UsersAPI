
namespace UserService.Core.Events
{
	public class UserCreatedEvent
	{
		public required int UserId { get; set; }
		public required string Email { get; set; }
	}
}
