using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics.CodeAnalysis;
using TechBlog.Application.Authentication.RefreshToken.Boundaries;
using TechBlog.Application.News.Create;
using TechBlog.Application.News.Create.Boundaries;
using TechBlog.Application.News.Delete;
using TechBlog.Application.News.Delete.Boundaries;
using TechBlog.Application.News.GetByStrategy;
using TechBlog.Application.News.GetByStrategy.Boundaries;
using TechBlog.Application.News.Update;
using TechBlog.Application.News.Update.Boundaries;
using TechBlog.Common.Api.Controllers;
using TechBlog.Common.Responses;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Identity;

namespace TechBlog.News.API.Controllers.V1
{

    [ApiVersion("1")]
    [ExcludeFromCodeCoverage]
    [SwaggerTag("Endpoints for news management")]
    public sealed class NewsController : ApiController
    {
        private readonly ICreateNewUseCase _createNewUseCase;
        private readonly IGetNewsByStrategyUseCase _getNewsByStrategyUseCase;
        private readonly IUpdateNewUseCase _updateNewUseCase;
        private readonly IDeleteNewUseCase _deleteNewUseCase;

        public NewsController(
            IConfiguration configuration,
            ICreateNewUseCase createNewUseCase,
            IGetNewsByStrategyUseCase getNewsByStrategyUseCase, 
            IUpdateNewUseCase updateNewUseCase, 
            IDeleteNewUseCase deleteNewUseCase) : base(configuration)
        {
            _createNewUseCase = createNewUseCase;
            _getNewsByStrategyUseCase = getNewsByStrategyUseCase;
            _updateNewUseCase = updateNewUseCase;
            _deleteNewUseCase = deleteNewUseCase;
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicies.IsJournalist)]
        [SwaggerOperation(
            Summary = "Creates a blog new",
            Description = "For more information, access our <a href='https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/66/POST-News'>wiki</a>.")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "New was successfully created", Type = typeof(CreateNewOutput))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid news information", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "Invalid access token or access key", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Server error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> Post([SwaggerRequestBody("News information")] CreateNewInput input, CancellationToken cancellationToken)
        {
            input.AddUser(HttpContext.User.GetBlogUserInformation());

            var output = await _createNewUseCase.CreateAsync(input, AsCombinedCancellationToken(cancellationToken));

            return CreatedAtAction(nameof(Post), output);
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Gets blog news by different strategies and parameters",
            Description = "For more information, access our <a href='https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/72/GET-News-by-Strategy'>wiki</a>.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "A list full of news", Type = typeof(GetNewsByStrategyOutput))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid request information", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "Invalid access token or access key", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Server error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetByStrategy([FromQuery]GetNewsByStrategyInput input, CancellationToken cancellationToken)
        {
            var output = await _getNewsByStrategyUseCase.GetAsync(input, cancellationToken);

            return Ok(output);
        }

        [HttpPatch("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Updates the information of a defined blog new",
            Description = "For more information, access our <a href='https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/68/PATCH-News-by-Id'>wiki</a>.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "New was updated successfully", Type = typeof(GetNewsByStrategyOutput))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid request information", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "Invalid access token or access key", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Server error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> Patch([SwaggerParameter("The blog new id")]string id, [SwaggerRequestBody("The new information to be updated")]UpdateNewInput input, CancellationToken cancellationToken)
        {
            input.Id = id;
            input.AddUser(HttpContext.User.GetBlogUserInformation());

            var output = await _updateNewUseCase.UpdateAsync(input, AsCombinedCancellationToken(cancellationToken));

            return Ok(output);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Deletes a defined blog new",
            Description = "For more information, access our <a href='https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/70/DELETE-News-by-Id'>wiki</a>.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Description = "New was deleted successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid request information", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "Invalid access token or access key", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Server error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> Delete([SwaggerParameter("The blog new id")] string id, CancellationToken cancellationToken)
        {
            var input = new DeleteNewInput(id);
            input.AddUser(HttpContext.User.GetBlogUserInformation());

            var output = await _deleteNewUseCase.DeleteAsync(input, AsCombinedCancellationToken(cancellationToken));

            if (output.Success)
                return NoContent();

            return Forbid();
        }
    }
}
