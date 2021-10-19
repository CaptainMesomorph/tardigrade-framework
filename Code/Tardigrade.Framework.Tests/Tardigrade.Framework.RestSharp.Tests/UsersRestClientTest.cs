using Tardigrade.Framework.Models.Rest;
using Tardigrade.Framework.RestSharp.Tests.Clients;
using Tardigrade.Framework.RestSharp.Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Tardigrade.Framework.RestSharp.Tests
{
    public class UsersRestClientTest
    {
        private readonly ITestOutputHelper output;

        public UsersRestClientTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test1()
        {
            var client = new UsersRestClient();
            Response<UserDto> response = client.Get<UserDto>("e6fc9cd91fd849d3");
            var content = response.Data?.ToString();
            output.WriteLine($"Status code: {response.StatusCode}\nContent: {content}");
        }
    }
}