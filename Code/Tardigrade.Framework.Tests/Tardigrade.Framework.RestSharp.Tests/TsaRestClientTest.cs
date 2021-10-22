using System;
using Tardigrade.Framework.Models.Rest;
using Tardigrade.Framework.RestSharp.Tests.Clients;
using Xunit;

namespace Tardigrade.Framework.RestSharp.Tests
{
    public class TsaRestClientTest
    {
        [Fact]
        public void Call_Lessons_Success()
        {
            var tsaClient = new TsaRestClient(new Uri("https://api.smoothlms.com/v1/"));
            Response<object> response = tsaClient.Get<object>("https://api.smoothlms.com/v1/enrolment/5095/lesson/6331/launch?returnURL=https%3A%2F%2Ftikforce.com");
            var content = response.Data?.ToString();
            Console.Out.WriteLine($"{response.StatusCode}\n{content}");
        }
    }
}