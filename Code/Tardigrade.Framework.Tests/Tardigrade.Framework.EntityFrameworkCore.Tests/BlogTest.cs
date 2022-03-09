using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using Tardigrade.Framework.EntityFrameworkCore.Extensions;
using Tardigrade.Framework.EntityFrameworkCore.Tests.SetUp;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Persistence;
using Tardigrade.Shared.Tests;
using Tardigrade.Shared.Tests.Models.Blogs;
using Xunit;
using Xunit.Abstractions;

namespace Tardigrade.Framework.EntityFrameworkCore.Tests;

public class BlogTest : IClassFixture<EntityFrameworkCoreClassFixture>
{
    private readonly IRepository<Blog, Guid> _blogRepository;
    private readonly EntityFrameworkCoreClassFixture _fixture;
    private readonly ITestOutputHelper _output;
    private readonly IRepository<Person, Guid> _personRepository;
    private readonly IRepository<Post, Guid> _postRepository;

    public BlogTest(EntityFrameworkCoreClassFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;

        _blogRepository = fixture.GetService<IRepository<Blog, Guid>>();
        _personRepository = fixture.GetService<IRepository<Person, Guid>>();
        _postRepository = fixture.GetService<IRepository<Post, Guid>>();

        // Get the current project's directory to store the create script.
        DirectoryInfo? binDirectory =
            Directory.GetParent(Directory.GetCurrentDirectory())?.Parent ??
            Directory.GetParent(Directory.GetCurrentDirectory());
        DirectoryInfo? projectDirectory = binDirectory?.Parent ?? binDirectory;
        string scriptDirectory = projectDirectory?.FullName ?? "c:\\temp";

        // Create and store SQL script for the test database.
        _fixture.GetService<DbContext>().GenerateCreateScript($"{scriptDirectory}\\Scripts\\TestDataCreateScript.sql");
        _fixture.PopulateDataStore();
    }

    [Fact]
    public void Delete_BlogExists_Success()
    {
        // Arrange.
        Blog retrieved = _blogRepository.Retrieve(_fixture.ReferenceBlog.Id, b => b.Posts);
        _output.WriteLine($"Blog to delete:\n{retrieved.ToJson()}");

        // Act.
        _blogRepository.Delete(retrieved);

        // Assert.
        bool blogExists = _blogRepository.Exists(_fixture.ReferenceBlog.Id);
        Assert.False(blogExists);
        bool postExists = _postRepository.Exists(_fixture.ReferenceBlog.Posts.First().Id);
        Assert.False(postExists);
        _output.WriteLine($"Successfully deleted blog {_fixture.ReferenceBlog.Id}.");
    }

    [Fact]
    public void Delete_NewBlog_Success()
    {
        // Arrange.
        Blog original = DataFactory.Blog;
        original.Posts = DataFactory.Posts;
        Blog created = _blogRepository.Create(original);
        _output.WriteLine($"Blog to delete:\n{created.ToJson()}");
        Assert.Equal(original.Id, created.Id);

        // Act.
        _blogRepository.Delete(created);

        // Assert.
        bool blogExists = _blogRepository.Exists(original.Id);
        Assert.False(blogExists);
        bool postExists = _postRepository.Exists(original.Posts.First().Id);
        Assert.False(postExists);
        _output.WriteLine($"Successfully deleted blog {original.Id}.");
    }

    [Fact]
    public void Delete_NewPerson_Success()
    {
        // Arrange.
        Person original = DataFactory.CreatePerson();
        Person created = _personRepository.Create(original);
        _output.WriteLine($"Person to delete:\n{created.ToJson()}");
        Assert.Equal(original.Id, created.Id);

        // Act.
        _personRepository.Delete(created);

        // Assert.
        bool personExists = _personRepository.Exists(original.Id);
        Assert.False(personExists);
        bool blogExists = _blogRepository.Exists(original.OwnedBlog.Id);
        Assert.False(blogExists);
        bool postExists = _postRepository.Exists(original.OwnedBlog.Posts.First().Id);
        Assert.False(postExists);
        _output.WriteLine($"Successfully deleted person {original.Id}.");
    }

    [Fact]
    public void Delete_OrphanObjects_Success()
    {
        // Arrange.
        Blog original = DataFactory.Blog;
        original.Posts = DataFactory.Posts;
        Blog created = _blogRepository.Create(original);
        _output.WriteLine($"Posts to delete:\n{created.Posts.ToJson()}");
        created.Posts.Clear();

        // Act.
        _blogRepository.Update(created);

        // Assert.
        bool blogExists = _blogRepository.Exists(original.Id);
        Assert.True(blogExists);

        foreach (Post post in original.Posts)
        {
            bool postExists = _postRepository.Exists(post.Id);
            Assert.False(postExists);
        }

        _output.WriteLine($"Successfully deleted orphan posts from blog {original.Id}.");
    }

    [Fact]
    public void Delete_PersonExists_Success()
    {
        // Arrange.
        Person retrieved = _personRepository.Retrieve(_fixture.ReferencePerson.Id, p => p.OwnedBlog.Posts);
        _output.WriteLine($"Person to delete:\n{retrieved.ToJson()}");

        // Act.
        _personRepository.Delete(retrieved);

        // Assert.
        bool personExists = _personRepository.Exists(_fixture.ReferencePerson.Id);
        Assert.False(personExists);
        bool blogExists = _blogRepository.Exists(_fixture.ReferencePerson.OwnedBlog.Id);
        Assert.False(blogExists);
        bool postExists = _postRepository.Exists(_fixture.ReferencePerson.OwnedBlog.Posts.First().Id);
        Assert.False(postExists);
        _output.WriteLine($"Successfully deleted person {_fixture.ReferencePerson.Id}.");
    }
}