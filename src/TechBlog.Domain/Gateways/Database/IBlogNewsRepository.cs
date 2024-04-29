using TechBlog.Domain.Entities;

namespace TechBlog.Domain.Gateways.Database
{
    public interface IBlogNewsRepository
    {
        Task<BlogNewEntity> AddAsync(BlogNewEntity entity, CancellationToken cancellationToken);
        Task DeleteAsync(string id, CancellationToken cancellationToken);
        Task<BlogNewEntity> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<IEnumerable<BlogNewEntity>> GetByCreatedDateAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
        Task<IEnumerable<BlogNewEntity>> GetByCreateOrUpdateDateAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
        Task<IEnumerable<BlogNewEntity>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<BlogNewEntity>> GetByTagsAsync(string[] tags, CancellationToken cancellationToken = default);
        Task<BlogNewEntity> UpdateAsync(BlogNewEntity blogNew, CancellationToken cancellationToken);
    }
}
