
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using TechBlog.Application.News.Delete;
using TechBlog.Application.News.Delete.Boundaries;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Gateways.Database;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.UnitTests.Application.News
{
    public sealed class DeleteNewsTest
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<DeleteNewInput> _validator;
        private readonly IBlogNewsRepository _repository;

        public DeleteNewsTest()
        {
            _logger =  Substitute.For<ILoggerManager>();
            _validator = Substitute.For<IValidator<DeleteNewInput>>();
            _repository = Substitute.For<IBlogNewsRepository>();
        }

        [Theory]
        [InlineData(true, true,null)]
        [InlineData(false, true, ResponseMessage.NotFound)]
        [InlineData(true, false, ResponseMessage.Forbidden)]
        public void Interactor_DeleteNews(bool active,bool isOwner, ResponseMessage? exceptionMessage = null)
        {
            try
            {
                //Arrange
                var id = Guid.NewGuid().ToString();
                var userId = Guid.NewGuid().ToString();
                var input = new DeleteNewInput(id)
                {
                    User = new TechBlog.Application.Common.Boundaries.BlogUserPort
                    {
                        BlogUserType = BlogUserType.JOURNALIST,
                        Id = userId,
                        Name = "Name",
                        Email = "email@test.com"
                    }
                };

                var blogNewEntity = new BlogNewEntity
                {
                    Id = id,
                    Enabled = active,
                    AuthorId = isOwner ? userId : Guid.NewGuid().ToString(),
                };

                _validator.ValidateAsync(Arg.Any<DeleteNewInput>())
                    .Returns(Task.FromResult(new ValidationResult()));

                _repository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(blogNewEntity));

                _repository.DeleteAsync(input.Id, Arg.Any<CancellationToken>())
                    .Returns(Task.CompletedTask);

                var sut = GenerateInteractor();

                //Act
                var output = sut.DeleteAsync(input, CancellationToken.None).Result;

                //Assert
                Assert.NotNull(output);
                output.Should().BeOfType<DeleteNewOutput>();
            }
            catch (Exception ex)
            {
                //Assert
                Assert.Equal(ex.InnerException.Message, exceptionMessage.ToString());
            }
            
        }

        private DeleteNewInteractor GenerateInteractor()
        {
            return new DeleteNewInteractor(_logger, _validator, _repository);
        }

    }
}
