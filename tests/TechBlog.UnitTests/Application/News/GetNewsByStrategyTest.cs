using Bogus;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechBlog.Application.News.GetByStrategy;
using TechBlog.Application.News.GetByStrategy.Boundaries;
using TechBlog.Application.News.GetByStrategy.Strategies;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Gateways.Database;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.UnitTests.Application.News
{
    public sealed class GetNewsByStrategyTest
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<GetNewsByStrategyInput> _validator;
        private readonly List<IGetNewsStrategy> _strategies;
        private readonly IBlogNewsRepository _repository;

        public GetNewsByStrategyTest()
        {
            _logger = Substitute.For<ILoggerManager>();
            _validator = Substitute.For<IValidator<GetNewsByStrategyInput>>();
            _strategies = new List<IGetNewsStrategy>();
            _repository = Substitute.For<IBlogNewsRepository>();
        }

        [Fact]
        public void Interactor_ValidInput_ShouldReturnRequest()
        {
            //Arrange
            var input = new GetNewsByStrategyInput
            {
                Strategy = GetNewsStrategy.GetById,
                Id = Guid.NewGuid().ToString()
            };

            _validator.ValidateAsync(Arg.Any<GetNewsByStrategyInput>(), Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _repository.GetByIdAsync(input.Id, Arg.Any<CancellationToken>())
                .Returns(new Faker<BlogNewEntity>().RuleFor(c => c.Enabled, true));

            _strategies.Add(new GetNewByIdStrategy(_logger, _repository));
            var sut = GetNewsByStrategyInteractor(_strategies);

            //Act
            var result = sut.GetAsync(input, CancellationToken.None).Result;

            // Assert
            Assert.NotNull(result);
            result.Should().BeOfType<GetNewsByStrategyOutput>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(1)]
        public void Interactor_InvalidInput_StrategyShouldBeThrowException(int qtdStrategy)
        {
            try
            {
                //Arrange
                var input = new GetNewsByStrategyInput
                {
                    Strategy = GetNewsStrategy.GetById,
                    Id = Guid.NewGuid().ToString()
                };

                _validator.ValidateAsync(Arg.Any<GetNewsByStrategyInput>(), Arg.Any<CancellationToken>())
                    .Returns(new FluentValidation.Results.ValidationResult());

                _repository.GetByIdAsync(input.Id, Arg.Any<CancellationToken>())
                    .Returns(new Faker<BlogNewEntity>().RuleFor(c => c.Enabled, true));

                switch (qtdStrategy)
                {
                    case 1:
                        _strategies.Add(new GetNewByIdStrategy(_logger, _repository));
                        break;
                    case 2:
                        _strategies.Add(new GetNewByIdStrategy(_logger, _repository));
                        _strategies.Add(new GetNewByIdStrategy(_logger, _repository));
                        break;
                }

                var sut = GetNewsByStrategyInteractor(_strategies);

                //Act
                var result = sut.GetAsync(input, CancellationToken.None).Result;

                // Assert
                Assert.NotNull(result);
                result.Should().BeOfType<GetNewsByStrategyOutput>();
            }
            catch (Exception ex)
            {
                // Assert
                switch (qtdStrategy)
                {
                    case 0:
                        Assert.Equal(ResponseMessage.StrategyIsNotImplemented.ToString(), ex.InnerException.Message);
                        break;
                    case 2:
                        Assert.Equal(ResponseMessage.StrategyHasMoreThanOneImplementation.ToString(), ex.InnerException.Message);
                        break;
                }
            }
            
        }

        private GetNewsByStrategyInteractor GetNewsByStrategyInteractor(List<IGetNewsStrategy> strategy = null)
        {
            return new GetNewsByStrategyInteractor(_logger, _validator, strategy != null ? strategy : _strategies);
        }

    }
}
