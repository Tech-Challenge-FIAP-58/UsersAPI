
namespace FCG.Core.Models
{
	public class UserProducerEventLog
	{
		public string Email { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public int UserId { get; set; }
		public DateTime DtCpu { get; set; }
	}
}
