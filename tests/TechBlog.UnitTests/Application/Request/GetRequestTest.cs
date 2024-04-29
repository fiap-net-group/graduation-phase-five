using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using TechBlog.Application.Request.GetRequest;
using TechBlog.Application.Request.GetRequest.Boundaries;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.Gateways.MemoryCache;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.UnitTests.Application.Request
{
    public sealed class GetRequestTest
    {
        private readonly ILoggerManager _logger;
        private readonly IMemoryCacheManager _memoryCache;
        private readonly IValidator<GetRequestInput> _validator;

        public GetRequestTest()
        {
            _logger = Substitute.For<ILoggerManager>();
            _memoryCache = Substitute.For<IMemoryCacheManager>();
            _validator = Substitute.For<IValidator<GetRequestInput>>();
        }

        [Fact]
        public void Interactor_ValidInput_ShouldReturnRequest()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var requestEntity = new RequestEntity
            {
                Status = RequestStatus.NotStarted,
                Message = ResponseMessage.Default
            };

            _validator.ValidateAsync(Arg.Any<GetRequestInput>(), CancellationToken.None)
                .Returns(new ValidationResult());
            _memoryCache.GetAsync<RequestEntity>(requestId, CancellationToken.None)
                .Returns(requestEntity);

            var sut = GenerateInteractor();

            // Act
            var output = sut.GetAsync(new GetRequestInput(requestId), CancellationToken.None).Result;

            // Assert
            output.Data.Status.Should().Be(requestEntity.Status);
            output.Data.Message.Should().Be(requestEntity.Message);
        }

        [Fact]
        public void Interactor_InvalidInput_ShouldBusinessException()
        {
            // Arrange
            var input = new GetRequestInput(Guid.NewGuid());

            _validator.ValidateAsync(Arg.Any<GetRequestInput>(), CancellationToken.None)
                .ThrowsAsync<BusinessException>();

            var sut = GenerateInteractor();

            // Act
            Func<Task> act = async () => await sut.GetAsync(input,CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<BusinessException>();
        }

        [Theory]
        [InlineData(RequestStatus.NotStarted)]
        [InlineData(RequestStatus.Processing)]
        [InlineData(RequestStatus.Completed)]
        [InlineData(RequestStatus.Canceled)]
        [InlineData(RequestStatus.InvalidInformation)]
        [InlineData(RequestStatus.InfrastructureError)]
        public void Interactor_Return_Different_Status(RequestStatus status)
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var requestEntity = new RequestEntity
            {
                Status = status,
                Message = ResponseMessage.Default
            };

            _validator.ValidateAsync(Arg.Any<GetRequestInput>(), CancellationToken.None)
                .Returns(new ValidationResult());
            _memoryCache.GetAsync<RequestEntity>(requestId, CancellationToken.None)
                .Returns(Task.FromResult(requestEntity));

            var sut = GenerateInteractor();

            // Act
            var output = sut.GetAsync(new GetRequestInput(requestId), CancellationToken.None).Result;

            // Assert
            output.Data.Status.Should().Be(requestEntity.Status);
            output.Data.Message.Should().Be(requestEntity.Message);
        }

        [Theory]
        [InlineData(StatusCodes.Status200OK)]
        [InlineData(StatusCodes.Status404NotFound)]
        [InlineData(StatusCodes.Status422UnprocessableEntity)]
        public void Interactor_Return_Different_StatusCode(int statusCode)
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var requestEntity = GetRequestEntity(requestId, statusCode);

            _validator.ValidateAsync(Arg.Any<GetRequestInput>(), CancellationToken.None)
                .Returns(new ValidationResult());
            _memoryCache.GetAsync<RequestEntity>(requestId, CancellationToken.None)
                .Returns(Task.FromResult(requestEntity));

            var sut = GenerateInteractor();

            // Act
            var output = sut.GetAsync(new GetRequestInput(requestId), CancellationToken.None).Result;

            // Assert
            output.StatusCode.Should().Be(statusCode);
        }

        private RequestEntity GetRequestEntity(Guid requestId, int statusCode)
        {
            var requestEntity = new RequestEntity
            {
                Status = RequestStatus.NotStarted,
            };

            if (statusCode.Equals(StatusCodes.Status404NotFound))
            {
                requestEntity = null;
            }

            if (statusCode.Equals(StatusCodes.Status422UnprocessableEntity))
            {
                requestEntity.Status = RequestStatus.InvalidInformation;
            }

            return requestEntity;
        }

        private GetRequestInteractor GenerateInteractor() => 
            new(_logger, _memoryCache, _validator);
    }
}
