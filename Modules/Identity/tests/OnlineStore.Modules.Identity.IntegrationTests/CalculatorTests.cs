using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace OnlineStore.Modules.Identity.IntegrationTests
{
    public class CalculatorTests
    {
        // Pass ITestOutputHelper into the test class, which xunit provides per-test
        public CalculatorTests(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        private ITestOutputHelper OutputHelper { get; }

        [Fact]
        public void Calculator_Sums_Two_Integers()
        {
            // Arrange - Create a service collection and call AddXunit()
            // on the logging builder to register it as a logging provider.
            var services = new ServiceCollection()
                .AddLogging((builder) => builder.AddXUnit(OutputHelper))
                .AddSingleton<Calculator>();

            // Get the system-under-test (the Calculator) from the service collection.
            // This will be created with a logger that routes to the xunit test output.
            var calculator = services
                .BuildServiceProvider()
                .GetRequiredService<Calculator>();

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