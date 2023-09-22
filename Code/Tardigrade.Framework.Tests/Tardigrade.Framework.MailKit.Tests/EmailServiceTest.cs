using System;
using Tardigrade.Framework.Emails;
using Tardigrade.Framework.MailKit.Tests.SetUp;
using Xunit;
using Xunit.Abstractions;

namespace Tardigrade.Framework.MailKit.Tests;

public class EmailServiceTest : IClassFixture<MailKitClassFixture>
{
    private readonly IEmailService _emailService;
    private readonly ITestOutputHelper _output;

    public EmailServiceTest(MailKitClassFixture fixture, ITestOutputHelper output)
    {
        _emailService = fixture.GetService<IEmailService>() ?? throw new InvalidOperationException();
        _output = output;
    }

    [Theory]
    [InlineData("tardigrade@mailinator.com")]
    public async void SendEmail_Valid_Success(string recipient)
    {
        const string subject = "Unit test";
        const string message = "<p>This is a unit test.</p>";

        // The sender email must be from the same domain as the SMTP user otherwise the SMTP server will reject it.
        var from = new EmailAddress { Address = "support@mailinator.com", Name = "Support" };
        var to = new EmailAddress { Address = recipient };
        await _emailService.SendMessageAsync(from, new[] { to }, subject, message);
        _output.WriteLine("Message successfully sent.");
    }
}