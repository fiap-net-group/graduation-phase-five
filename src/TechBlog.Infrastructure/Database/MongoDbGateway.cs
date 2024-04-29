using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using TechBlog.Infrastructure.Database.Configuration;

namespace TechBlog.Infrastructure.Database
{
    public sealed class MongoDbGateway : IMongoDbGateway
    {
        private readonly IMongoDatabase _client;

        public MongoDbGateway(IConfiguration configuration)
        {
            var settings = MongoClientSettings.FromConnectionString(configuration.GetValue("Gateways:MongoDb:ConnectionString", ""));

            _client = new MongoClient(settings).GetDatabase(configuration.GetValue("Gateways:MongoDb:DatabaseName", ""));
        }

        public IMongoCollection<TCollection> GetCollection<TCollection>(string collectionName) 
        {
            ArgumentNullException.ThrowIfNull(collectionName);

            return _client.GetCollection<TCollection>(collectionName);
        }
    }
}
