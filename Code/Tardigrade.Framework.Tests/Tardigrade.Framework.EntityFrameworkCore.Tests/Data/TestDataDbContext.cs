using System;
using Microsoft.EntityFrameworkCore;
using Tardigrade.Shared.Tests.Models;
using Tardigrade.Shared.Tests.Models.Blogs;

namespace Tardigrade.Framework.EntityFrameworkCore.Tests.Data
{
    /// <summary>
    /// Database context for a database containing test data.
    /// </summary>
    internal class TestDataDbContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<CredentialIssuer> CredentialIssuers { get; set; }
        public DbSet<Credential> Credentials { get; set; }
        public DbSet<UserCredential> UserCredentials { get; set; }
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="options">Options associated with the database context.</param>
        public TestDataDbContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Person>()
            //    .HasOne(p => p.OwnedBlog)
            //    .WithOne(b => b.Owner)
            //    .HasForeignKey<Blog>("OwnerId");

            //modelBuilder
            //    .Entity<Blog>()
            //    .Property<Guid>("OwnerId");

            modelBuilder
                .Entity<Blog>()
                .HasOne(b => b.Owner)
                .WithOne(p => p.OwnedBlog)
                .OnDelete(DeleteBehavior.ClientCascade);

            //modelBuilder
            //    .Entity<Post>()
            //    .HasOne(pt => pt.Author)
            //    .WithMany(pn => pn.Posts)
            //    .HasForeignKey(pt => pt.AuthorId);

            modelBuilder
                .Entity<Post>()
                .HasOne(p => p.Blog)
                .WithMany(b => b.Posts)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}