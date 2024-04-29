using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics.CodeAnalysis;
using TechBlog.Application.User.ChangePassword;
using TechBlog.Application.User.ChangePassword.Boundaries;
using TechBlog.Application.User.Create;
using TechBlog.Application.User.Create.Boundaries;
using TechBlog.Application.User.Delete;
using TechBlog.Application.User.Delete.Boundaries;
using TechBlog.Application.User.GetById;
using TechBlog.Application.User.GetById.Boundaries;
using TechBlog.Application.User.Update;
using TechBlog.Application.User.Update.Boundaries;
using TechBlog.Common.Api.Controllers;
using TechBlog.Common.Responses;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Extensions;

namespace TechBlog.Users.API.Controllers.V1
{
    [ApiVersion("1")]
    [ExcludeFromCodeCoverage]
    [SwaggerTag("Endpoints for users management")]
    public sealed class UsersController : ApiController
    {
        private readonly ICreateUserUseCase _createUserUseCase;
        private readonly IUpdateUserUseCase _updateUserUseCase;
        private readonly IGetUserByIdUseCase _getUserByIdUseCase;
        private readonly IDeleteUserUseCase _deleteUserUseCase;
        private readonly IChangePasswordUseCase _changePasswordUseCase;

        public UsersController(
            IConfiguration configuration,
            ICreateUserUseCase createUserUseCase,
            IUpdateUserUseCase updateUserUseCase,
            IGetUserByIdUseCase getUserByIdUseCase,
            IDeleteUserUseCase deleteUserUseCase,
            IChangePasswordUseCase changePasswordUseCase) : base(configuration)
        {
            _createUserUseCase = createUserUseCase;
            _updateUserUseCase = updateUserUseCase;
            _getUserByIdUseCase = getUserByIdUseCase;
            _deleteUserUseCase = deleteUserUseCase;
            _changePasswordUseCase = changePasswordUseCase;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new User", 
            Description = "For more information, access our <a href='https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/52/POST-User'>wiki</a>.")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "User created", Type = typeof(CreateUserOutput))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid information", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "Invalid access key", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Server error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> Post([SwaggerRequestBody("User information")] CreateUserInput input, CancellationToken cancellationToken)
        {
            var output = await _createUserUseCase.CreateAsync(input, AsCombinedCancellationToken(cancellationToken));

            return CreatedAtAction(nameof(Post), output);
        }

        [Authorize]
        [HttpPatch("{id}")]
        [SwaggerOperation(
            Summary = "Updates a User",
            Description = "For more information, access our <a href='https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/56/PATCH-User-by-id'>wiki</a>.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "User updated", Type = typeof(UpdateUserOutput))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid information", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "Invalid access token or access key", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, Description = "User id is not the same as the token", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Server error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> Patch([SwaggerParameter("The user id", Required = true)] string id, [SwaggerRequestBody("User information")] UpdateUserInput input, CancellationToken cancellationToken)
        {
            if (id != HttpContext.User.GetUserId())
                throw new ForbiddenException();

            input.Id = id;

            var output = await _updateUserUseCase.UpdateAsync(input, AsCombinedCancellationToken(cancellationToken));

            return Ok(output);
        }

        [Authorize]
        [HttpPatch("{id}/change-password")]
        [SwaggerOperation(
            Summary = "Changes the User password", 
            Description = "For more information, access our <a href='https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/59/PATCH-Change-Password'>wiki</a>.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Password updated", Type = typeof(UpdateUserOutput))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid information", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "Invalid access token or access key", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, Description = "User id is not the same as the token", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Server error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> PatchChangePassword([SwaggerParameter("The user id", Required = true)] string id, [SwaggerRequestBody("New password")] ChangePasswordInput input, CancellationToken cancellationToken)
        {
            if (id != HttpContext.User.GetUserId())
                throw new ForbiddenException();

            input.Id = id;

            var output = await _changePasswordUseCase.ChangePasswordAsync(input, AsCombinedCancellationToken(cancellationToken));

            return Ok(output);
        }

        [Authorize]
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Gets a User by id", 
            Description = "For more information, access our <a href='https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/57/GET-User-by-id'>wiki</a>.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "User updated", Type = typeof(GetUserByIdOutput))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid information", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "Invalid access token or access key", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, Description = "User id is not the same as the token", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Server error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetById([SwaggerParameter("The user id", Required = true)] string id, CancellationToken cancellationToken)
        {
            if (id != HttpContext.User.GetUserId())
                throw new ForbiddenException();

            var output = await _getUserByIdUseCase.GetByIdAsync(new GetUserByIdInput(id), cancellationToken);
            
            return Ok(output);
        }

        [Authorize]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Deletes a User", 
            Description = "For more information, access our <a href='https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/58/DELETE-User-by-id'>wiki</a>.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Description = "User deleted")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid information", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "Invalid access token or access key", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, Description = "User id is not the same as the token", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Server error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> Delete([SwaggerParameter("The user id", Required = true)] string id, CancellationToken cancellationToken)
        {
            if (id != HttpContext.User.GetUserId())
                throw new ForbiddenException();

            await _deleteUserUseCase.DeleteAsync(new DeleteUserInput(id), cancellationToken);

            return NoContent();
        }
    }
}
