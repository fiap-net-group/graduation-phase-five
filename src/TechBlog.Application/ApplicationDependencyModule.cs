using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TechBlog.Application.Authentication.Login;
using TechBlog.Application.Authentication.RefreshToken;
using TechBlog.Application.Email.Send;
using TechBlog.Application.News.Create;
using TechBlog.Application.News.Create.Boundaries;
using TechBlog.Application.News.Delete;
using TechBlog.Application.News.GetByStrategy;
using TechBlog.Application.News.GetByStrategy.Strategies;
using TechBlog.Application.News.Update;
using TechBlog.Application.Request.CreateRequest;
using TechBlog.Application.Request.CreateRequest.Boundaries;
using TechBlog.Application.Request.GetRequest;
using TechBlog.Application.Request.UpdateRequestStatus;
using TechBlog.Application.User.ChangePassword;
using TechBlog.Application.User.Create;
using TechBlog.Application.User.Create.Boundaries;
using TechBlog.Application.User.Delete;
using TechBlog.Application.User.GetById;
using TechBlog.Application.User.Reactivate;
using TechBlog.Application.User.Update;

namespace TechBlog.Application;

public static class ApplicationDependencyModule
{
    public static IServiceCollection AddNotificationApplicationConfiguration(this IServiceCollection services)
    {
        services.AddScoped<ICreateRequestUseCase, CreateRequestInteractor>();
        services.AddScoped<IGetRequestUseCase, GetRequestInteractor>();
        services.AddScoped<IUpdateRequestStatusUseCase, UpdateRequestStatusInteractor>();

        services.AddValidatorsFromAssemblyContaining<CreateRequestValidator>();

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        return services;
    }

    public static IServiceCollection AddNewsApplicationConfiguration(this IServiceCollection services)
    {
        services.AddScoped<ICreateNewUseCase, CreateNewInteractor>();
        services.AddScoped<IGetNewsByStrategyUseCase, GetNewsByStrategyInteractor>();
        services.AddScoped<IUpdateNewUseCase, UpdateNewInteractor>();
        services.AddScoped<IDeleteNewUseCase, DeleteNewInteractor>();

        services.AddScoped<IGetNewsStrategy, GetNewsByTagStrategy>();
        services.AddScoped<IGetNewsStrategy, GetNewsByNameStrategy>();
        services.AddScoped<IGetNewsStrategy, GetNewsByCreateOrUpdateDateStrategy>();
        services.AddScoped<IGetNewsStrategy, GetNewsByCreateDateStrategy>();
        services.AddScoped<IGetNewsStrategy, GetNewByIdStrategy>();

        services.AddValidatorsFromAssemblyContaining<CreateNewValidator>();
                
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        return services;
    }

    public static IServiceCollection AddUsersApplicationConfiguration(this IServiceCollection services)
    {
        services.AddScoped<ICreateUserUseCase, CreateUserInteractor>();
        services.AddScoped<IUpdateUserUseCase, UpdateUserInteractor>();
        services.AddScoped<IGetUserByIdUseCase, GetUserByIdInteractor>();
        services.AddScoped<IDeleteUserUseCase, DeleteUserInteractor>();
        services.AddScoped<IChangePasswordUseCase, ChangePasswordInteractor>();
        services.AddScoped<IReactivateUserUseCase, ReactivateUserInteractor>();

        services.AddScoped<ILoginUseCase, LoginInteractor>();
        services.AddScoped<IRefreshTokenUseCase, RefreshTokenInteractor>();

        services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        return services;
    }

    public static IServiceCollection AddSendEmailWorkerApplicationConfiguration(this IServiceCollection services)
    {
        services.AddScoped<ISendEmailUseCase, SendEmailInteractor>();

        services.AddValidatorsFromAssemblyContaining<CreateRequestValidator>();

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        return services;
    }
}
