using Microsoft.Extensions.DependencyInjection;
using Tardigrade.Framework.Emails;
using Tardigrade.Framework.MailKit.Tests.SetUp;
using Xunit;
using Xunit.Abstractions;

namespace Tardigrade.Framework.MailKit.Tests
{
    public class EmailServiceTest : IClassFixture<MailKitClassFixture>
    {
        private readonly IEmailService _emailService;
        private readonly ITestOutputHelper _output;

        public EmailServiceTest(MailKitClassFixture fixture, ITestOutputHelper output)
        {
            _emailService = fixture.Services.GetService<IEmailService>();
            _output = output;
        }

        [Theory]
        [InlineData("raf@mailinator.com")]
        public async void Test1(string recipient)
        {
            const string subject = "Unit test";
            const string message = "<p>This is a unit test.</p>";

            // The sender email must be from the credenxia.com domain otherwise the SMTP server will reject it.
            var from = new EmailAddress { Address = "support@credenxia.com", Name = "Support" };
            var to = new EmailAddress { Address = recipient };
            await _emailService.SendMessageAsync(from, new[] { to }, subject, message);
            _output.WriteLine("Message successfully sent.");
        }
    }
}