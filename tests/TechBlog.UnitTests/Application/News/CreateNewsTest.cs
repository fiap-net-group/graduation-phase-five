using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using TechBlog.Application.News.Create;
using TechBlog.Application.News.Create.Boundaries;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Gateways.Database;
using TechBlog.Domain.Gateways.Integrations.UsersApi;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;
using TechBlog.UnitTests.Fixtures;

namespace TechBlog.UnitTests.Application.News
{
    public sealed class CreateNewsTest
    {
        private readonly UnitTestsFixture _fixture;

        private readonly ILoggerManager _logger;
        private readonly IValidator<CreateNewInput> _validator;
        private readonly IBlogNewsRepository _repository;
        private readonly IUsersApi _usersApi;

        public CreateNewsTest()
        {
            _logger = Substitute.For<ILoggerManager>();
            _validator = Substitute.For<IValidator<CreateNewInput>>();
            _repository = Substitute.For<IBlogNewsRepository>();
            _usersApi = Substitute.For<IUsersApi>();
            _fixture = new UnitTestsFixture();
        }

        [Theory]
        [InlineData(BlogUserType.READER, false, ResponseMessage.Forbidden)]
        [InlineData(BlogUserType.JOURNALIST, false, ResponseMessage.UserDontExists)]
        [InlineData(BlogUserType.JOURNALIST, true, null)]
        public void Interactor_Return_Different_StatusCode(BlogUserType blogUserType, bool expectedSuccess, ResponseMessage? exceptionMessage = null)
        {
            try
            {
                // Arrange
                var id = Guid.NewGuid().ToString();
                var input = new CreateNewInput
                {
                    Title = "Title",
                    Description = "Description",
                    Body = "Body",
                    Tags = new string[] { "tag1", "tag2" },
                    Enabled = true,
                    User = new TechBlog.Application.Common.Boundaries.BlogUserPort
                    {
                        BlogUserType = blogUserType,
                        Id = id,
                        Name = "Name",
                        Email = "email@test.com"
                    }
                };

                _validator.ValidateAsync(Arg.Any<CreateNewInput>(), CancellationToken.None)
                    .Returns(new ValidationResult());

                _usersApi.ExistsByIdAsync(Arg.Any<string>(), CancellationToken.None)
                    .Returns(expectedSuccess);

                _repository.AddAsync(Arg.Any<BlogNewEntity>(), CancellationToken.None)
                    .Returns(new BlogNewEntity
                    {
                        //AuthorId = _userContext.UserId,
                        Title = input.Title,
                        Description = input.Description,
                        Body = input.Body,
                        Tags = input.Tags,
                        Enabled = input.Enabled
                    });

                var sut = GetCreateNewInteractor();

                // Act
                var result = sut.CreateAsync(input, CancellationToken.None).Result;

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<CreateNewOutput>();
                
            }
            catch(Exception ex)
            {
                // Assert
                Assert.Equal(ex.InnerException.Message, exceptionMessage.ToString());
            }
        }

        [Theory]
        [InlineData("Title", "Description", "Body", new string[] { "tag1", "tag2" }, true, true, new string[] { })]
        [InlineData(null, null, null, new string[] { }, true, false, new string[] { "InvalidTitle", "InvalidDescription", "InvalidBody", "InvalidTags" })]
        [InlineData("", "", "", new string[] { }, true, false, new string[] { "InvalidTitle", "InvalidDescription","InvalidBody","InvalidTags" })]
        [InlineData("", null, null, new string[] { }, false, false, new string[] { "InvalidTitle", "InvalidDescription", "InvalidBody", "InvalidTags" })]
        [InlineData("", "", null, new string[] { }, false, false, new string[] { "InvalidTitle", "InvalidDescription", "InvalidBody", "InvalidTags" })]
        [InlineData("", "", "", new string[] { }, false, false, new string[] { "InvalidTitle", "InvalidDescription", "InvalidBody", "InvalidTags" })]
        [InlineData("Title", "", "", new string[] { }, false, false, new string[] { "InvalidDescription", "InvalidBody", "InvalidTags" })]
        [InlineData("Title", "Description", "", new string[] { }, false, false, new string[] { "InvalidBody", "InvalidTags" })]
        [InlineData("Title", "Description", "Body", new string[] { }, false, false, new string[] { "InvalidTags" })]

        public void Validator_AllCases_ShouldMatchResponse(string title, string description, string body, string[] tags, bool enabled, bool expectedValid, string[] responseMessages)
        {
            // Arrange
            var input = new CreateNewInput
            {
                Title = title,
                Description = description,
                Body = body,
                Tags = tags,
                Enabled = enabled
            };

            _validator.ValidateAsync(Arg.Any<CreateNewInput>(), CancellationToken.None)
                .Returns(new ValidationResult());

            var sut = new CreateNewValidator();

            // Act
            var result = sut.Validate(input);

            // Assert
            result.IsValid.Should().Be(expectedValid);
            result.Errors.Select(e => e.ErrorMessage).Should().BeEquivalentTo(responseMessages);

        }

        private CreateNewInteractor GetCreateNewInteractor()
        {
            return new CreateNewInteractor(_logger, _validator, _repository, _usersApi);
        }
    }
}
