using MongoDB.Driver;

namespace TechBlog.Infrastructure.Database.Configuration
{
    public interface IMongoDbGateway
    {
        IMongoCollection<TCollection> GetCollection<TCollection>(string collectionName);
    }
}
