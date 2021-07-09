using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Tests.Integration.Constants;
using Common.Tests.Integration.Fixtures;
using Common.Utils.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Api;
using OnlineStore.Modules.Identity.Application.Features.System;
using OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser;
using OnlineStore.Modules.Identity.Infrastructure;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;
using Xunit;
using Xunit.Abstractions;

namespace OnlineStore.Modules.Identity.IntegrationTests.UserRegistrations
{
    [Collection(nameof(IntegrationTestFixture<Startup, IdentityDbContext>))]
    public sealed class UserRegistrationTests
    {
        private readonly IntegrationTestFixture<Startup, IdentityDbContext> _fixture;

        public UserRegistrationTests(IntegrationTestFixture<Startup, IdentityDbContext> fixture,
            ITestOutputHelper outputHelper)
        {
            _fixture = fixture;
            _fixture.SetOutput(outputHelper);
            var user = _fixture.CreateAdminUserMock();

            //setup the swaps for our tests
            _fixture.RegisterTestServices(services =>
            {
                services.ReplaceScoped<IDataSeeder, IdentityTestSeeder>();
                services.AddScoped(_ => user);
            });
        }

        [Fact]
        public async Task RegisterNewUserCommand_Test()
        {
            //Arrange
            var registerUserCommand = new RegisterNewUserCommand(UserRegistrationSampleData.Id.BindId(),
                UserRegistrationSampleData.Email,
                UserRegistrationSampleData.FirstName, UserRegistrationSampleData.LastName,
                UserRegistrationSampleData.Name,
                UserRegistrationSampleData.UserName, UserRegistrationSampleData.Password,
                UserRegistrationSampleData.Permissions.ToImmutableList(), UserRegistrationSampleData.UserType,
                UserRegistrationSampleData.IsAdministrator, UserRegistrationSampleData.IsActive,
                UserRegistrationSampleData.Roles.ToImmutableList(), UserRegistrationSampleData.LockoutEnabled,
                UserRegistrationSampleData.EmailConfirmed, UserRegistrationSampleData.PhotoUrl,
                UserRegistrationSampleData.Status);

            //Act
            await _fixture.SendAsync(registerUserCommand);

            //Assert
            // var query = new GetUserByIdQuery(registerUserCommand.Id);
            var userManager = _fixture.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var created = await userManager.FindByIdAsync(registerUserCommand.Id.ToString());
            // var created = await _fixture.QueryAsync(query);

            created.Should().NotBeNull();
            created.Id.Should().Be(registerUserCommand.Id.ToString());
            created.CreatedBy.Should().BeEquivalentTo(UsersConstants.AdminUserMock.UserName);
            created.Roles.Should().NotBeNull();
            created.Roles.Select(role => role.Name).Should().BeEquivalentTo(UserRegistrationSampleData.Roles);
            created.Permissions.Select(permission => permission.Name).Should()
                .BeEquivalentTo(UserRegistrationSampleData.Permissions);

            var messagesList = await _fixture.OutboxMessagesHelper.GetOutboxMessages();
            messagesList.Count.Should().Be(1);
            var newUserRegisteredNotification =
                await _fixture.OutboxMessagesHelper.GetLastOutboxMessage<NewUserRegisteredNotification>();
            newUserRegisteredNotification.DomainEvent.User.UserName.Should().Be(UserRegistrationSampleData.UserName);
        }
    }
}