using Tardigrade.Shared.Tests;
using Tardigrade.Shared.Tests.Extensions;
using Tardigrade.Shared.Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Tardigrade.Framework.Tests
{
    public class JsonSerialisationTest
    {
        private readonly ITestOutputHelper output;

        public JsonSerialisationTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Convert_New_Success()
        {
            User user = DataFactory.User;
            string str = user.ToSystemTextJson();
            output.WriteLine($"JSON is {str}.");
        }
    }
}