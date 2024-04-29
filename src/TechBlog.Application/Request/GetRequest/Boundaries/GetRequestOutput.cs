using TechBlog.Domain.Entities;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.Request.GetRequest.Boundaries
{
    [ExcludeFromCodeCoverage]
    public sealed class GetRequestOutput
    {
        public RequestEntity Data { get; set; }
        public int StatusCode { get; set; }

        public GetRequestOutput(RequestEntity data)
        {
            Data = data;

            if (data == null)
            {
                StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            if (data.Status == RequestStatus.InvalidInformation)
            {
                StatusCode = StatusCodes.Status422UnprocessableEntity;
                return;
            }

            StatusCode = StatusCodes.Status200OK;
        }
    }
}