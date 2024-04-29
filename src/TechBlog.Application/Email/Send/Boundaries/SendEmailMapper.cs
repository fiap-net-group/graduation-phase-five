using FluentValidation.Results;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Gateways.Email;
using Mapster;
using System.Diagnostics.CodeAnalysis;
using TechBlog.Application.Request.CreateRequest.Boundaries;
using TechBlog.Application.Request.UpdateRequestStatus.Boundaries;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.Email.Send.Boundaries
{
    [ExcludeFromCodeCoverage]
    public class SendEmailMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<SendEmailInput, CreateRequestInput>()
               .Map(destination => destination.OperationName, source => nameof(SendEmailEvent))
               .Map(destination => destination.Value, source => (object)source);

            config.NewConfig<CreateRequestInput, SendEmailEvent>()

                .Map(destination => destination.RequestId, source => Guid.Empty)
                .Map(destination => destination.Value, source => (SendEmailInput)source.Value);

            config.NewConfig<ValidationResult, SendEmailOutput>()
                .ConstructUsing(source => new SendEmailOutput(new RequestEntity
                (
                    Enum.Parse<ResponseMessage>(source.Errors[0].ErrorMessage),
                    RequestStatus.InvalidInformation
                )));

            config.NewConfig<SendEmailInput, EmailData>()
                .Map(destination => destination.To, source => source.To)
                .Map(destination => destination.HtmlBody, source => source.Body)
                .Map(destination => destination.Subject, source => source.Subject);

            config.NewConfig<SendEmailOutput, UpdateRequestStatusEvent>()                
                .Map(destination => destination.RequestId, source => source.RequestId)
                .Map(destination => destination.Value, source => new UpdateRequestStatusInput(source.Value));
        }
    }
}
