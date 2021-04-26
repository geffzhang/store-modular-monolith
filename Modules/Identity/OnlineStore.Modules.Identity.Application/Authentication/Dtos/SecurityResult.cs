using System.Collections.Generic;

namespace OnlineStore.Modules.Identity.Application.Authentication.Dtos
{
    public class SecurityResult
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}