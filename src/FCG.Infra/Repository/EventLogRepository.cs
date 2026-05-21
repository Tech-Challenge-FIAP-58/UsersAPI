using FCG.Core.Models;
using FCG.Infra.Context;
using MongoDB.Driver;

namespace FCG.Infra.Repository
{
	public class EventLogRepository(MongoDbService mongoDbService) : IEventLogRepository
	{
		private readonly IMongoCollection<UserProducerEventLog>? _userProducerEventLogs = mongoDbService.Database?.GetCollection<UserProducerEventLog>("userProducerLogs");

		public async Task InsertUserProducerEventLog(UserProducerEventLog log)
		{
			if (_userProducerEventLogs is null) return;
			await _userProducerEventLogs.InsertOneAsync(log);
		}
	}
}
