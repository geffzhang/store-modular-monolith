using System;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common;
using Common.Tests.Integration.Constants;
using Common.Tests.Integration.Fixtures;
using Common.Tests.Integration.Mocks;
using Common.Utils.Extensions;
using Common.Web.Contexts;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using OnlineStore.Modules.Identity.Api;
using OnlineStore.Modules.Identity.Application.Features.System;
using OnlineStore.Modules.Identity.Application.Features.Users.GetUserById;
using OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser;
using OnlineStore.Modules.Identity.Infrastructure;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;
using Xunit;
using Xunit.Abstractions;

namespace OnlineStore.Modules.Identity.IntegrationTests.UserRegistrations
{
    [Collection(nameof(IntegrationTestFixture<Startup, IdentityDbContext>))]
    public class UserRegistrationTests
    {
        private readonly IntegrationTestFixture<Startup, IdentityDbContext> _fixture;

        public UserRegistrationTests(IntegrationTestFixture<Startup, IdentityDbContext> fixture,
            ITestOutputHelper outputHelper)
        {
            _fixture = fixture;
            _fixture.SetOutput(outputHelper);
            //setup the swaps for our tests
            _fixture.RegisterTestServices(services =>
            {
                services.ReplaceScoped<IDataSeeder, IdentityTestSeeder>();
                services.AddSingleton<Calculator>();
                var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
                httpContextAccessorMock.HttpContext = new DefaultHttpContext
                {
                    RequestServices = services.BuildServiceProvider()
                };
                services.ReplaceScoped(_ => httpContextAccessorMock);
                var res = httpContextAccessorMock.HttpContext.AuthenticateAsync(AuthConstants.Scheme).GetAwaiter()
                    .GetResult();
                httpContextAccessorMock.HttpContext.User = res.Ticket?.Principal!;
                services.ReplaceScoped<IExecutionContextAccessor>(x =>
                    new ExecutionContextAccessorMock(CreateExecutionContext(x.GetService<IHttpContextAccessor>())));
            });
        }

        private ExecutionContext CreateExecutionContext(IHttpContextAccessor httpContextAccessor)
        {
            var context = new ExecutionContextFactory(httpContextAccessor).Create();
            return context;
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
        }

        // [Fact]
        // public async Task RegisterNewUserCommand_Test()
        // {
        //     //Arrange
        //     var registerUserCommand = new RegisterNewUserCommand(UserRegistrationSampleData.Id.BindId(),
        //         UserRegistrationSampleData.Email,
        //         UserRegistrationSampleData.FirstName, UserRegistrationSampleData.LastName,
        //         UserRegistrationSampleData.Name,
        //         UserRegistrationSampleData.UserName, UserRegistrationSampleData.Password,
        //         UserRegistrationSampleData.Permissions.ToImmutableList(), UserRegistrationSampleData.UserType,
        //         UserRegistrationSampleData.IsAdministrator, UserRegistrationSampleData.IsActive,
        //         UserRegistrationSampleData.Roles.ToImmutableList(), UserRegistrationSampleData.LockoutEnabled,
        //         UserRegistrationSampleData.EmailConfirmed, UserRegistrationSampleData.PhotoUrl,
        //         UserRegistrationSampleData.Status);
        //
        //     //Act
        //     await _fixture.SendAsync(registerUserCommand);
        //
        //     //Assert
        //     var query = new GetUserByIdQuery(registerUserCommand.Id);
        //     var created = await _fixture.QueryAsync(query);
        //     
        //     created.Should().NotBeNull();
        //     created.Id.Should().Be(registerUserCommand.Id);
        //     created.CreatedBy.Should().BeEquivalentTo(UsersConstants.AdminUser.UserName);
        //     created.Roles.Should().NotBeNull();
        //     created.Roles.Select(x => x).Should().BeEquivalentTo(UserRegistrationSampleData.Roles);
        //     created.Permissions.Select(x => x).Should().BeEquivalentTo(UserRegistrationSampleData.Permissions);
        //
        //     var messagesList = await _fixture.OutboxMessagesHelper.GetOutboxMessages();
        //     messagesList.Count.Should().Be(1);
        //     var newUserRegisteredNotification =
        //         await _fixture.OutboxMessagesHelper.GetLastOutboxMessage<NewUserRegisteredNotification>();
        //     newUserRegisteredNotification.DomainEvent.User.UserName.Should().Be(UserRegistrationSampleData.UserName);
        // }
        
        [Fact]
        public void Calculator_Sums_Two_Integers()
        {
            // Get the system-under-test (the Calculator) from the service collection.
            // This will be created with a logger that routes to the xunit test output.
            var calculator = _fixture.ServiceProvider .GetRequiredService<Calculator>();

            // Act
            int actual = calculator.Sum(1, 2);

            // Assert
            3.Should().Be(actual);
        }
    }
    
    public sealed class Calculator
    {
        private readonly ILogger _logger;

        public Calculator(ILogger<Calculator> logger)
        {
            _logger = logger;
        }

        public int Sum(int x, int y)
        {
            int sum = x + y;

            _logger.LogInformation("The sum of {x} and {y} is {sum}.", x, y, sum);

            return sum;
        }
    }
}