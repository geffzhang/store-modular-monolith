using System.Collections.Generic;

namespace Common.Identity
{
    public class SecurityResult
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
