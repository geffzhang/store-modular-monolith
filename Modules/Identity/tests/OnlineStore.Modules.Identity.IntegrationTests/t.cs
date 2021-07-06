using System;
using Xunit;
using Xunit.Abstractions;

namespace OnlineStore.Modules.Identity.IntegrationTests
{
    public class XUnitTestClass
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public XUnitTestClass(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            _testOutputHelper.WriteLine("Hello");
        }
    }
}