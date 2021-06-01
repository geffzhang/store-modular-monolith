using System.Collections.Generic;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Dtos.UseCaseResponses
{
    public class UserDetailDto
    {
        public string Id { get; set; }
        public IList<string> Permissions { get; set; } = new List<string>();
        public string UserName { get; set; }
        public bool IsAdministrator { get; set; }
        public bool PasswordExpired { get; set; }
        public int DaysTillPasswordExpiry { get; set; }
    }
}