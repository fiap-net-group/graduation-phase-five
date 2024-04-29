using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Entities.Definitions;

namespace TechBlog.Infrastructure.Database.Configuration
{
    public sealed class MongoDbBlogNewCollection : IBlogNewEntity<ObjectId>
    {
        public ObjectId Id { get; set; }
        public string ShardKey { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public bool Enabled { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        [BsonIgnoreIfNull]
        public string[] Tags { get; set; }

        public MongoDbBlogNewCollection(BlogNewEntity source)
        {
            Id = ObjectId.GenerateNewId();
            ShardKey = ShardKeyGenerator.GenerateShardKey(source.Title);
            Title = source.Title;
            Description = source.Description;
            Body = source.Body;
            Enabled = source.Enabled;
            AuthorId = source.AuthorId;
            Tags = source.Tags;
            CreatedAt = source.CreatedAt;
            LastUpdateAt = source.LastUpdateAt;
        }

        public BlogNewEntity AsEntity()
        {
            return new BlogNewEntity
            {
                Id = Id.ToString(),
                Title = Title,
                Description = Description,
                Body = Body,
                Enabled = Enabled,
                AuthorId = AuthorId,
                Tags = Tags,
                CreatedAt = CreatedAt,
                LastUpdateAt = LastUpdateAt,
            };
        }
    }
}
