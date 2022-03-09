using Tardigrade.Framework.Extensions;
using Tardigrade.Shared.Tests;
using Tardigrade.Shared.Tests.Models.Credentials;
using Xunit;
using Xunit.Abstractions;

namespace Tardigrade.Framework.Tests;

public class JsonSerialisationTest
{
    private readonly ITestOutputHelper _output;

    public JsonSerialisationTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Convert_New_Success()
    {
        User user = DataFactory.User;
        string str = user.ToJson();
        _output.WriteLine($"JSON is {str}.");
    }
}