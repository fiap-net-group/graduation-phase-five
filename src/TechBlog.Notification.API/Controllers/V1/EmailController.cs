using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechBlog.Application.Email.Send.Boundaries;
using TechBlog.Application.News.Create.Boundaries;
using TechBlog.Application.Request.CreateRequest;
using TechBlog.Application.Request.CreateRequest.Boundaries;
using TechBlog.Application.Request.GetRequest;
using TechBlog.Application.Request.GetRequest.Boundaries;
using TechBlog.Common;
using TechBlog.Common.Responses;
using TechBlog.Notification.API.Controllers.Core;

namespace TechBlog.Notification.API.Controllers.V1;

[ApiVersion("1.0")]
[ExcludeFromCodeCoverage]
public class EmailController : EventDrivenApiController
{
    public EmailController(
        IConfiguration configuration,
        ICreateRequestUseCase createRequest,
        IGetRequestUseCase getRequest) : base(configuration, createRequest, getRequest) { }

    [HttpPost]
    [SwaggerOperation(
            Summary = "Creates a email send process",
            Description = "For more information, access our <a href='https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/66/POST-News'>wiki</a>.")]
    [SwaggerResponse(StatusCodes.Status202Accepted, Description = "Email process was created", Type = typeof(CreateNewOutput))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid request body", Type = typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Server error", Type = typeof(ErrorResponse))]
    public async Task<IActionResult> PostSend([SwaggerRequestBody("The e-mail information")] SendEmailInput input, CancellationToken cancellationToken) =>
        await CreateRequest<SendEmailEvent, SendEmailInput>(input, cancellationToken);

    [HttpGet]
    [SwaggerOperation(
            Summary = "Gets a request by id",
            Description = "For more information, access our <a href='https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/66/POST-News'>wiki</a>.")]
    [SwaggerResponse(StatusCodes.Status200OK, Description = "Email was sent successfully", Type = typeof(GetRequestOutput))]
    [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Request was not found", Type = typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, Description = "Invalid business rules", Type = typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Server error", Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetSend([FromQuery] GetRequestInput input, CancellationToken cancellationToken) =>
        await GetRequest(input, cancellationToken);
}

