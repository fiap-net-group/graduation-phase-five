using MongoDB.Bson;
using MongoDB.Driver;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Gateways.Database;
using TechBlog.Infrastructure.Database.Configuration;

namespace TechBlog.Infrastructure.Database.Repositories
{
    public sealed class BlogNewsRepository : IBlogNewsRepository
    {
        private readonly IMongoDbGateway _mongoDbGateway;

        public BlogNewsRepository(IMongoDbGateway mongoDbGateway)
        {
            _mongoDbGateway = mongoDbGateway;
        }

        private IMongoCollection<MongoDbBlogNewCollection> Collection =>
            _mongoDbGateway.GetCollection<MongoDbBlogNewCollection>(nameof(BlogNewEntity));

        public async Task<BlogNewEntity> AddAsync(BlogNewEntity entity, CancellationToken cancellationToken)
        {
            var entityToInsert = new MongoDbBlogNewCollection(entity);

            await Collection.InsertOneAsync(entityToInsert, new InsertOneOptions { BypassDocumentValidation = false }, cancellationToken);

            entity.Id = entityToInsert.Id.ToString();

            return entity;
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            _ = ObjectId.TryParse(id, out var objectId);

            var update = Builders<MongoDbBlogNewCollection>.Update.Set(entity => entity.Enabled, false);

            await Collection.FindOneAndUpdateAsync(filter => filter.Id == objectId, update, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<BlogNewEntity>> GetByCreatedDateAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            var entities = await (await Collection.FindAsync(filter => 
                filter.CreatedAt >= from && filter.CreatedAt <= to, cancellationToken: cancellationToken)).ToListAsync(cancellationToken);

            return entities.Select(entity => entity.AsEntity());
        }

        public async Task<IEnumerable<BlogNewEntity>> GetByCreateOrUpdateDateAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            var entities = await (await Collection.FindAsync(filter =>
                (filter.CreatedAt >= from && filter.CreatedAt <= to) ||
                (filter.LastUpdateAt >= from && filter.LastUpdateAt <= to), cancellationToken: cancellationToken)).ToListAsync(cancellationToken);

            return entities.Select(entity => entity.AsEntity());
        }

        public async Task<BlogNewEntity> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            _ = ObjectId.TryParse(id, out var objectId);

            var entity = await (await Collection.FindAsync(filter => filter.Id == objectId, cancellationToken: cancellationToken)).SingleOrDefaultAsync(cancellationToken);

            return entity == null ?
                new BlogNewEntity
                {
                    Enabled = false
                } :
                entity.AsEntity();
        }

        public async Task<IEnumerable<BlogNewEntity>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var entities = await (await Collection.FindAsync(filter => 
                filter.Title.ToLower() == name.ToLower(), cancellationToken: cancellationToken)).ToListAsync(cancellationToken);

            return entities.Select(entity => entity.AsEntity());
        }

        public async Task<IEnumerable<BlogNewEntity>> GetByTagsAsync(string[] tags, CancellationToken cancellationToken = default)
        {
            var filter = Builders<MongoDbBlogNewCollection>.Filter.ElemMatch(entity => entity.Tags, tag => tags.Contains(tag));

            var entities = await(await Collection.FindAsync(filter, cancellationToken: cancellationToken)).ToListAsync(cancellationToken);

            return entities.Select(entity => entity.AsEntity());
        }

        public async Task<BlogNewEntity> UpdateAsync(BlogNewEntity blogNew, CancellationToken cancellationToken)
        {
            _ = ObjectId.TryParse(blogNew.Id, out var id);

            var update = Builders<MongoDbBlogNewCollection>.Update.Set(entity => entity.Title, blogNew.Title)
                                                                  .Set(entity => entity.Description, blogNew.Description)
                                                                  .Set(entity => entity.Body, blogNew.Body);

            await Collection.FindOneAndUpdateAsync(filter => filter.Id == id, update, cancellationToken: cancellationToken);

            return blogNew;
        }
    }
}
