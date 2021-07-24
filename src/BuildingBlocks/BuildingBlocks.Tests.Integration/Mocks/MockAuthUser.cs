using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace BuildingBlocks.Tests.Integration.Mocks
{
    public class MockAuthUser
    {
        public List<Claim> Claims { get; }

        public MockAuthUser(params Claim[] claims) => Claims = claims.ToList();
    }
}