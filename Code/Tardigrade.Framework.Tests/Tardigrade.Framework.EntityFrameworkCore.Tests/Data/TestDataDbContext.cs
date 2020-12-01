using Microsoft.EntityFrameworkCore;
using Tardigrade.Shared.Tests.Models;

namespace Tardigrade.Framework.EntityFrameworkCore.Tests.Data
{
    /// <summary>
    /// Database context for a database containing test data.
    /// </summary>
    internal class TestDataDbContext : DbContext
    {
        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="options">Options associated with the database context.</param>
        public TestDataDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CredentialIssuer> CredentialIssuers { get; set; }
        public DbSet<Credential> Credentials { get; set; }
        public DbSet<UserCredential> UserCredentials { get; set; }
        public DbSet<User> Users { get; set; }
    }
}