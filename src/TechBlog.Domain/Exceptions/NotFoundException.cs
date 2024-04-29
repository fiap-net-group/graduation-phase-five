using TechBlog.Domain.ValueObjects;

namespace TechBlog.Domain.Exceptions
{
    public class NotFoundException : BusinessException
    {
        public NotFoundException() : base(ResponseMessage.NotFound.ToString()) { }

        public NotFoundException(string message) : base(ResponseMessage.NotFound.ToString(), message)
        {

        }

        public NotFoundException(string message, params string[] errors) : base(ResponseMessage.Forbidden.ToString())
        {
            errors ??= Array.Empty<string>();

            var newErrors = new string[errors.Length + 1];

            newErrors[0] = message;

            for (int i = 1; i < errors.Length; i++)
                newErrors[i] = errors[i - 1];

            Errors = newErrors;
        }
    }
}
