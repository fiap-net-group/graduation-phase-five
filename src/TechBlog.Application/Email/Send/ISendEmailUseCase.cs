using TechBlog.Application.Email.Send.Boundaries;

namespace TechBlog.Application.Email.Send
{
    public interface ISendEmailUseCase
    {
        Task<SendEmailOutput> SendAsync(SendEmailInput input, CancellationToken cancellationToken);
    }
}
