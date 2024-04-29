using Mapster;
using TechBlog.Application.User.Reactivate.Boundaries;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Integrations.NotificationsApi.PostEmail;

namespace TechBlog.Application.User.Create.Boundaries
{
    public class CreateUserMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateUserInput, BlogUserEntity>()
                            .Map(destination => destination.Id, source => Guid.NewGuid().ToString())
                            .Map(destination => destination.CreatedAt, source => DateTime.UtcNow)
                            .Map(destination => destination.LastUpdateAt, source => DateTime.UtcNow)
                            .Map(destination => destination.Email, source => source.Email)
                            .Map(destination => destination.Name, source => source.Name)
                            .Map(destination => destination.Password, source => source.Password)
                            .Map(destination => destination.BlogUserType, source => source.BlogUserType)
                            ;

            config.NewConfig<AccessTokenModel, CreateUserOutput>()
                .ConstructUsing(source => new CreateUserOutput(source))
                ;

            config.NewConfig<BlogUserEntity, ReactivateUserInput>()
                .ConstructUsing(source => new ReactivateUserInput(source.Id, source.Password))
                ;

            config.NewConfig<ReactivateUserOutput, CreateUserOutput>()
                .ConstructUsing(source => new CreateUserOutput(source))
                ;

            config.NewConfig<CreateUserInput, PostEmailRequest>()
                .ConstructUsing(source => new PostEmailRequest(source.Email, EmailExtensions.EmailSubject, EmailExtensions.EmailMessage))
                ;
        }
    }
}
