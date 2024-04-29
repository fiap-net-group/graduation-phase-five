using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace TechBlog.Common.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiVersion("1.0")]
    [ExcludeFromCodeCoverage]
    public class ApiController : ControllerBase
    {
        private readonly int _cancelRequisitionAfterInSeconds;

        public ApiController(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            _cancelRequisitionAfterInSeconds = configuration.GetValue<int>("CancelRequisitionAfterInSeconds", 30);
        }

        protected CancellationToken AsCombinedCancellationToken(CancellationToken requestCancellationToken)
        {
            using var combinedCancellationTokens = CancellationTokenSource.CreateLinkedTokenSource(requestCancellationToken, HttpContext.RequestAborted);

            combinedCancellationTokens.CancelAfter(_cancelRequisitionAfterInSeconds);

            return combinedCancellationTokens.Token;
        }
    }
}
