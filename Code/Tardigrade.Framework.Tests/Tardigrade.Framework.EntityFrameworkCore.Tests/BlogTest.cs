﻿using System;
using System.Linq;
using Tardigrade.Framework.EntityFrameworkCore.Tests.SetUp;
using Tardigrade.Framework.Persistence;
using Tardigrade.Shared.Tests;
using Tardigrade.Shared.Tests.Extensions;
using Tardigrade.Shared.Tests.Models.Blogs;
using Xunit;
using Xunit.Abstractions;

namespace Tardigrade.Framework.EntityFrameworkCore.Tests
{
    public class BlogTest : IClassFixture<UnitTestFixture>
    {
        private readonly UnitTestFixture fixture;
        private readonly ITestOutputHelper output;
        private readonly IRepository<Blog, Guid> blogRepository;
        private readonly IRepository<Person, Guid> personRepository;
        private readonly IRepository<Post, Guid> postRepository;

        public BlogTest(UnitTestFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.output = output;

            blogRepository = fixture.Container.GetService<IRepository<Blog, Guid>>();
            personRepository = fixture.Container.GetService<IRepository<Person, Guid>>();
            postRepository = fixture.Container.GetService<IRepository<Post, Guid>>();
        }

        [Fact]
        public void Delete_NewObject_Success()
        {
            // Arrange.
            Blog original = DataFactory.Blog;
            Blog created = blogRepository.Create(original);
            output.WriteLine($"Blog to delete:\n{created.ToJson()}");
            Assert.Equal(original.Id, created.Id);

            // Act.
            blogRepository.Delete(created);

            // Assert.
            bool blogExists = blogRepository.Exists(original.Id);
            Assert.False(blogExists);
            bool postExists = postRepository.Exists(original.Posts.First().Id);
            Assert.False(postExists);
            output.WriteLine($"Successfully deleted blog {original.Id}.");
        }

        [Fact]
        public void Delete_NewPerson_Success()
        {
            // Arrange.
            Person original = DataFactory.CreatePerson();
            Person created = personRepository.Create(original);
            output.WriteLine($"Person to delete:\n{created.ToJson()}");
            Assert.Equal(original.Id, created.Id);
            Person retrieved = personRepository.Retrieve(created.Id, p => p.OwnedBlog);

            // Act.
            personRepository.Delete(retrieved);

            // Assert.
            bool personExists = personRepository.Exists(original.Id);
            Assert.False(personExists);
            bool blogExists = blogRepository.Exists(original.OwnedBlog.Id);
            Assert.False(blogExists);
            bool postExists = postRepository.Exists(original.Posts.First().Id);
            Assert.False(postExists);
            output.WriteLine($"Successfully deleted person {original.Id}.");
        }

        [Fact]
        public void Delete_ObjectExists_Success()
        {
            // Arrange.
            Blog retrieved = blogRepository.Retrieve(fixture.ReferenceBlog.Id);
            output.WriteLine($"Blog to delete:\n{retrieved.ToJson()}");

            // Act.
            blogRepository.Delete(retrieved);

            // Assert.
            bool blogExists = blogRepository.Exists(fixture.ReferenceBlog.Id);
            Assert.False(blogExists);
            bool postExists = postRepository.Exists(fixture.ReferenceBlog.Posts.First().Id);
            Assert.False(postExists);
            output.WriteLine($"Successfully deleted blog {fixture.ReferenceBlog.Id}.");
        }

        [Fact]
        public void Delete_OrphanObjects_Success()
        {
            // Arrange.
            Blog original = DataFactory.Blog;
            Blog created = blogRepository.Create(original);
            output.WriteLine($"Posts to delete:\n{created.Posts.ToJson()}");
            created.Posts.Clear();

            // Act.
            blogRepository.Update(created);

            // Assert.
            bool blogExists = blogRepository.Exists(original.Id);
            Assert.True(blogExists);

            foreach (Post post in original.Posts)
            {
                bool postExists = postRepository.Exists(post.Id);
                Assert.False(postExists);
            }

            output.WriteLine($"Successfully deleted orphan posts from blog {original.Id}.");
        }
    }
}