using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Reflection;
using Tardigrade.Framework.AuditNET.Decorators;
using Tardigrade.Framework.AzureStorage.Configurations;
using Tardigrade.Framework.Persistence;
using Tardigrade.Framework.Services;
using Tardigrade.Framework.Services.Users;
using Tardigrade.Framework.Testing;
using Tardigrade.Shared.Tests;
using Tardigrade.Shared.Tests.Models.Credentials;
using Tardigrade.Shared.Tests.Models.Passwords;

namespace Tardigrade.Framework.AuditNET.Tests.SetUp
{
    public class AuditClassFixture : UnitTestClassFixture
    {
        protected override Assembly EntryAssembly => Assembly.GetExecutingAssembly();

        public Password ReferencePassword { get; } = DataFactory.Password;

        public User ReferenceUser { get; } = DataFactory.User;

        protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // Mock required services.
            // The read-only repositories are only required for the Update operations of the Decorator. As the Update
            // operations are not called for these tests, no mock implementations need be provided.
            // https://stackoverflow.com/questions/32791287/mock-offoo-setting-return-for-method-calls-with-parameters

            var userContext = Mock.Of<IUserContext>();
            var passwordRepository = Mock.Of<IReadOnlyRepository<Password, Guid>>();
            var passwordService = Mock.Of<IObjectService<Password, Guid>>();
            var userRepository = Mock.Of<IReadOnlyRepository<User, Guid>>();
            var userService = Mock.Of<IObjectService<User, Guid>>();

            Mock.Get(userContext).Setup(service => service.CurrentUser).Returns("Raf");
            Mock.Get(passwordService)
                .Setup(service => service.Create(It.IsAny<Password>()))
                .Returns<Password>(password => password);
            Mock.Get(userService).Setup(service => service.Create(It.IsAny<User>())).Returns<User>(user => user);

            // Inject services.

            services.Configure<IConfiguration>(context.Configuration);

            services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));

            services.AddTransient(_ => passwordRepository);
            services.AddTransient(_ => passwordService);
            services.AddTransient(_ => userContext);
            services.AddTransient(_ => userRepository);
            services.AddTransient(_ => userService);
            services.AddTransient<IAzureStorageConfiguration, AzureStorageConfiguration>();

            services.Decorate<IObjectService<Password, Guid>, ObjectServiceAuditDecorator<Password, Guid>>();
            services.Decorate<IObjectService<User, Guid>, ObjectServiceAuditDecorator<User, Guid>>();
        }
    }
}