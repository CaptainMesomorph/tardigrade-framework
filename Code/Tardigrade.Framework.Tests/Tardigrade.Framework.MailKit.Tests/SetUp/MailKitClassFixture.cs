using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Tardigrade.Framework.Emails;
using Tardigrade.Framework.MailKit.Configurations;
using Tardigrade.Framework.MailKit.Emails;
using Tardigrade.Framework.MailKit.Profiles;
using Tardigrade.Framework.Testing;

namespace Tardigrade.Framework.MailKit.Tests.SetUp
{
    public class MailKitClassFixture : UnitTestClassFixture
    {
        protected override Assembly EntryAssembly => Assembly.GetExecutingAssembly();

        protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MailKitProfile));
            services.Configure<IConfiguration>(context.Configuration);
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IMailKitConfiguration, MailKitConfiguration>();
        }
    }
}