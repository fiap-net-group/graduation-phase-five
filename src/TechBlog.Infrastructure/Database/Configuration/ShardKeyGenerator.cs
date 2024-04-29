namespace TechBlog.Infrastructure.Database.Configuration
{
    public static class ShardKeyGenerator
    {
        public static string GenerateShardKey(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
                return field;

            return string.Format("{0}{1}", field.ToUpper()[0], field.Length);
        }
    }
}
