namespace TechBlog.Application.News.GetByStrategy.Strategies
{
    public enum GetNewsStrategy
    {
        GetById,
        GetByCreateDate,
        GetByCreateOrUpdateDate,
        GetByTags,
        GetByName
    }
}
