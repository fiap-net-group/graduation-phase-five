namespace TechBlog.Common.Responses
{
    public sealed class ResponseDetails
    {
        public string[] Errors { get; set; }
        public int ErrorCount { get; set; }

        public ResponseDetails(string[] errors = null)
        {
            Errors = errors ?? Array.Empty<string>();
            ErrorCount = Errors.Length;
        }
    }
}
