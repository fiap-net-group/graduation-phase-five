using TechBlog.Common.Responses;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Common;

public class BaseResponse
    {
        private ResponseMessage ResponseMessage { get; set; }
        public bool Success { get; set; }
        public ResponseDetails ResponseDetails { get; set; }

        public BaseResponse AsError(ResponseMessage? message = null, params string[] errors)
        {
            errors ??= Array.Empty<string>();
            Success = false;
            ResponseMessage = message ?? ResponseMessage.UnexpectedError;
            ResponseDetails = new ResponseDetails
            {
                ErrorCount = errors.Length,
                Errors = errors
            };
            return this;
        }

        public BaseResponse AsSuccess()
        {
            Success = true;
            ResponseMessage = ResponseMessage.Default;
            ResponseDetails = new ResponseDetails
            {
                ErrorCount = 0,
                Errors = default
            };
            return this;
        }

        public ResponseMessage GetMessage()
        {
            return ResponseMessage;
        }
    }
