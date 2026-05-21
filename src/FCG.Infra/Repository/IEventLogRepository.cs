using FCG.Core.Models;

namespace FCG.Infra.Repository
{
	public interface IEventLogRepository
	{
		Task InsertUserProducerEventLog(UserProducerEventLog log);
	}
}
