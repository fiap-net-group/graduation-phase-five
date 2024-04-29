using TechBlog.Domain.ValueObjects;

namespace TechBlog.Domain.Exceptions
{
    public class ForbiddenException : BusinessException
    {
        public ForbiddenException() : base(ResponseMessage.Forbidden.ToString()) { }

        public ForbiddenException(string message) : base(ResponseMessage.Forbidden.ToString(), message) 
        { 
        
        }

        public ForbiddenException(string message, params string[] errors) : base(ResponseMessage.Forbidden.ToString())
        {
            errors ??= Array.Empty<string>();

            var newErrors = new string[errors.Length + 1];

            newErrors[0] = message;

            for(int i = 1; i < errors.Length; i++)            
                newErrors[i] = errors[i-1];            

            Errors = newErrors;
        }
    }
}
