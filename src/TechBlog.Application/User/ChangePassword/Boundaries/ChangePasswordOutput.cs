using Mapster;
using TechBlog.Application.Common.Boundaries;
using TechBlog.Application.User.GetById.Boundaries;
using TechBlog.Domain.Entities;

namespace TechBlog.Application.User.ChangePassword.Boundaries
{
    public sealed class ChangePasswordOutput : BlogUserPort 
    {
        public ChangePasswordOutput()
        {
            TypeAdapterConfig<BlogUserEntity, GetUserByIdOutput>
                .NewConfig()
                .Map(destination => destination.Id, source => source.Id)
                .Map(destination => destination.Name, source => source.Name)
                .Map(destination => destination.BlogUserType, source => source.BlogUserType)
                .Map(destination => destination.Email, source => source.Email)
                ;
        }
    }
}
