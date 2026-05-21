using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace FCG.Infra.Context
{
	public class MongoDbService
	{
		private readonly IConfiguration _configuration;
		private readonly IMongoDatabase? _database;

		public MongoDbService(IConfiguration configuration)
		{
			_configuration = configuration;

			var connectionString = configuration.GetConnectionString("MongoDb");
			if (!string.IsNullOrWhiteSpace(connectionString))
			{
				var mongoUrl = MongoUrl.Create(connectionString);
				var mongoClient = new MongoClient(mongoUrl);
				_database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
			}
		}

		public IMongoDatabase? Database => _database;
	}
}
