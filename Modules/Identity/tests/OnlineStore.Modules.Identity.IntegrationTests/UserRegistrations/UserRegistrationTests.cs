using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Messaging.Commands;
using Common.Utils.Extensions;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Modules.Identity.Application.Features.Users.RegisterNewUser;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace OnlineStore.Modules.Identity.IntegrationTests.UserRegistrations
{
    [Collection(nameof(IdentityIntegrationTestFixture))]
    public class UserRegistrationTests
    {
        private readonly IdentityIntegrationTestFixture _fixture;

        public UserRegistrationTests(IdentityIntegrationTestFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _fixture.SetOutput(output);
        }

        [Fact]
        public async Task RegisterNewUserCommand_Test()
        {
            //Arrange
            var registerUserCommand = new RegisterNewUserCommand(UserRegistrationSampleData.Id.BindId(), UserRegistrationSampleData.Email,
                UserRegistrationSampleData.FirstName, UserRegistrationSampleData.LastName, UserRegistrationSampleData.Name,
                UserRegistrationSampleData.UserName, UserRegistrationSampleData.Password,
                UserRegistrationSampleData.Permissions.ToImmutableList(), UserRegistrationSampleData.UserType,
                UserRegistrationSampleData.IsAdministrator, UserRegistrationSampleData.IsActive,
                UserRegistrationSampleData.Roles.ToImmutableList(), UserRegistrationSampleData.LockoutEnabled,
                UserRegistrationSampleData.EmailConfirmed, UserRegistrationSampleData.PhotoUrl, UserRegistrationSampleData.Status);

            //TODO: UnitOfWork Decorator for repository
            //Act
            await _fixture.SendAsync(registerUserCommand);

            //Assert
            var appUser = await _fixture.FindAsync<ApplicationUser>(registerUserCommand.Id);

            appUser.ShouldNotBeNull();
            appUser.Id.ShouldBeEquivalentTo(registerUserCommand.Id.ToString());
            appUser.Roles.ShouldNotBeNull();
            appUser.Roles.Select(x => x.Name).ShouldAllBe(x => UserRegistrationSampleData.Roles.Contains(x));
            appUser.Permissions.Select(x => x.Name).ShouldAllBe(x => UserRegistrationSampleData.Permissions.Contains(x));
            
            var messagesList = await _fixture.OutboxMessagesHelper.GetOutboxMessages();
            messagesList.Count.ShouldBe(1);
            var newUserRegisteredNotification = await _fixture.OutboxMessagesHelper.GetLastOutboxMessage<NewUserRegisteredNotification>();
            newUserRegisteredNotification.DomainEvent.User.UserName.ShouldBe(UserRegistrationSampleData.UserName);
        }
    }
}