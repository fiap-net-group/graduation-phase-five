using System.Diagnostics.CodeAnalysis;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using TechBlog.Application.Request.CreateRequest;
using TechBlog.Application.Request.CreateRequest.Boundaries;
using TechBlog.Application.Request.GetRequest;
using TechBlog.Application.Request.GetRequest.Boundaries;
using TechBlog.Domain.Gateways.Event;
using TechBlog.Common.Responses;
using TechBlog.Common.Api.Controllers;

namespace TechBlog.Notification.API.Controllers.Core;

[ExcludeFromCodeCoverage]
public class EventDrivenApiController : ApiController
{
    private readonly ICreateRequestUseCase _createRequest;
    private readonly IGetRequestUseCase _getRequest;

    public EventDrivenApiController(IConfiguration configuration, ICreateRequestUseCase createRequest, IGetRequestUseCase getRequest) : base(configuration)
    {
        _createRequest = createRequest;
        _getRequest = getRequest;
    }

    protected async Task<IActionResult> CreateRequest<TEvent, TInput>(TInput input, CancellationToken cancellationToken) where TEvent : BaseEvent<TInput>
    {
        var output = await _createRequest.CreateAsync<TEvent, TInput>(input.Adapt<CreateRequestInput>(), AsCombinedCancellationToken(cancellationToken));

        var response = new BaseResponseWithValue<CreateRequestOutput>().AsSuccess(output);

        return Accepted(response);
    }

    protected async Task<IActionResult> GetRequest<T>(T input, CancellationToken cancellationToken)
    {
        var output = await _getRequest.GetAsync(input.Adapt<GetRequestInput>(), AsCombinedCancellationToken(cancellationToken));

        var response = new BaseResponseWithValue<GetRequestOutput>().AsSuccess(output);

        return StatusCode(response.Value.StatusCode, response);
    }
}
