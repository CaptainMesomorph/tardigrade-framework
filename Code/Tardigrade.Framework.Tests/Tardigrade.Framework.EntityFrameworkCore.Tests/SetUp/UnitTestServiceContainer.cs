using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tardigrade.Framework.Configurations;
using Tardigrade.Framework.EntityFrameworkCore.Tests.Data;
using Tardigrade.Framework.Patterns.DependencyInjection;
using Tardigrade.Framework.Persistence;
using Tardigrade.Shared.Tests.Models;
using Tardigrade.Shared.Tests.Models.Blogs;

namespace Tardigrade.Framework.EntityFrameworkCore.Tests.SetUp
{
    internal class UnitTestServiceContainer : MicrosoftServiceContainer
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            var config = new ApplicationConfiguration();
            string connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<TestDataDbContext>(options => options.UseSqlite(connectionString));

            // Inject business services.
            services.AddScoped<DbContext, TestDataDbContext>();
            services.AddScoped<IRepository<Blog, Guid>, Repository<Blog, Guid>>();
            services.AddScoped<IRepository<Person, Guid>, Repository<Person, Guid>>();
            services.AddScoped<IRepository<Post, Guid>, Repository<Post, Guid>>();
            services.AddScoped<IRepository<User, Guid>, Repository<User, Guid>>();
            services.AddScoped<IRepository<UserCredential, Guid>, Repository<UserCredential, Guid>>();
        }
    }
}