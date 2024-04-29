using Swashbuckle.AspNetCore.Annotations;
using TechBlog.Application.News.GetByStrategy.Strategies;

namespace TechBlog.Application.News.GetByStrategy.Boundaries
{
    public sealed class GetNewsByStrategyInput
    {
        [SwaggerParameter("The strategy for the query")]
        public GetNewsStrategy Strategy { get; set; }
        [SwaggerParameter("The new id")]
        public string Id { get; set; }
        [SwaggerParameter("The new name")]
        public string Name { get; set; }
        [SwaggerParameter("The new tags")]
        public string[] Tags { get; set; }
        [SwaggerParameter("Initial date for query")]
        public DateTime? From { get; set; }
        [SwaggerParameter("Final date for query")]
        public DateTime? To { get; set; }

        public bool ValidId() => !string.IsNullOrWhiteSpace(Id);
        public bool ValidName() => !string.IsNullOrWhiteSpace(Name);
        public bool ValidTags() => Tags != null && Tags.Length > 0;
        public bool ValidDateInterval() => To != DateTime.MinValue &&
                                         From != DateTime.MinValue &&
                                         From <= To;
    }
}
