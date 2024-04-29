using TechBlog.Domain.ValueObjects;

namespace TechBlog.Common.Responses
{
    public sealed class ErrorResponse
    {
        public ResponseMessage Message { get; set; }
        public ResponseDetails ResponseDetails { get; set; }

        public ErrorResponse(ResponseMessage? message = null, params string[] errors)
        {
            errors ??= Array.Empty<string>();
            Message = message ?? ResponseMessage.UnexpectedError;
            ResponseDetails = new ResponseDetails(errors);
        }
    }
}
