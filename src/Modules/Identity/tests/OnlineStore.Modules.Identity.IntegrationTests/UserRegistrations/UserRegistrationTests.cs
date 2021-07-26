using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Tests.Integration.Constants;
using BuildingBlocks.Tests.Integration.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.API;
using OnlineStore.Modules.Identity.Application.System;
using OnlineStore.Modules.Identity.Application.Users.Features.GetUserById;
using OnlineStore.Modules.Identity.Application.Users.Features.RegisterNewUser;
using OnlineStore.Modules.Identity.Infrastructure;
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
            var registerUserCommand = new RegisterNewUserCommand(
                UserRegistrationSampleData.Email,
                UserRegistrationSampleData.FirstName,
                UserRegistrationSampleData.LastName,
                UserRegistrationSampleData.Name,
                UserRegistrationSampleData.UserName,
                UserRegistrationSampleData.PhoneNumber,
                UserRegistrationSampleData.Password,
                UserRegistrationSampleData.Permissions.ToImmutableList(),
                UserRegistrationSampleData.UserType,
                UserRegistrationSampleData.IsAdministrator,
                UserRegistrationSampleData.IsActive,
                UserRegistrationSampleData.Roles.ToImmutableList(),
                UserRegistrationSampleData.LockoutEnabled,
                UserRegistrationSampleData.EmailConfirmed,
                UserRegistrationSampleData.PhotoUrl,
                UserRegistrationSampleData.Status);

            //async operation test simulation
            // var tsc = _fixture.EnsureReceivedMessageToConsumer(registerUserCommand);
            // await _fixture.PublishAsync(registerUserCommand); // send command asynchronously
            // await tsc.Task;

            await _fixture.SendAsync(registerUserCommand); // send command synchronously
            var tsc = _fixture.EnsureReceivedMessageToConsumer<NewUserRegisteredIntegrationEvent>();
            await tsc.Task;

            //Assert
            var query = new GetUserByIdQuery(registerUserCommand.Id);
            var created = await _fixture.QueryAsync(query);

            created.Should().NotBeNull();
            created.Id.Should().Be(registerUserCommand.Id.ToString());
            created.CreatedBy.Should().BeEquivalentTo(UsersConstants.AdminUserMock.UserName);
            created.Roles.Should().NotBeNull();
            created.Roles.Select(role => role).Should().BeEquivalentTo(UserRegistrationSampleData.Roles);
            created.Permissions.Select(permission => permission).Should()
                .BeEquivalentTo(UserRegistrationSampleData.Permissions);
            var messagesList = await _fixture.OutboxMessagesHelper.GetOutboxMessages();
            messagesList.Count.Should().Be(1);
            var newUserRegisteredNotification =
                await _fixture.OutboxMessagesHelper.GetLastOutboxMessage<NewUserRegisteredNotification>();
            newUserRegisteredNotification.DomainEvent.User.UserName.Should().Be(UserRegistrationSampleData.UserName);
        }
    }
}