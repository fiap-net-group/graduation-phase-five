using System.Runtime.Serialization;

namespace TechBlog.Domain.Exceptions
{
    [Serializable]
    public class BusinessException : Exception
    {
        public string[] Errors { get; set; }
        public BusinessException() { }

        public BusinessException(string message) : base(message) { }

        public BusinessException(string message, params string[] errors) : base(message)
        {
            errors ??= Array.Empty<string>();
            Errors = errors;
        }

        protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }

        public static void ThrowIfNull<T>(T obj)
        {
            if (obj is null)
                throw new BusinessException("Invalid parameter");
        }
    }
}
