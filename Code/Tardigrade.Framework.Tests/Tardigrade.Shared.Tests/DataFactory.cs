using Bogus;
using System.Collections.Generic;
using Tardigrade.Shared.Tests.Extensions;
using Tardigrade.Shared.Tests.Models;
using Tardigrade.Shared.Tests.Models.Blogs;
using Person = Tardigrade.Shared.Tests.Models.Blogs.Person;

namespace Tardigrade.Shared.Tests
{
    public static class DataFactory
    {
        private const int BlogMaxCount = 4;
        private const int CredentialMaxCount = 4;
        private const int CredentialIssuerMaxCount = 4;
        private const int PersonMaxCount = 4;
        private const int PostMaxCount = 4;
        private const int UserMaxCount = 4;
        private const int UserCredentialMaxCount = 4;

        private const int BlogSeed = 01234;
        private const int CredentialSeed = 12345;
        private const int CredentialIssuerSeed = 23456;
        private const int PersonSeed = 34567;
        private const int PostSeed = 45678;
        private const int UserSeed = 56789;
        private const int UserCredentialSeed = 67890;

        private static readonly Faker<Blog> BlogFaker;
        private static readonly Faker<Credential> CredentialFaker;
        private static readonly Faker<CredentialIssuer> CredentialIssuerFaker;
        private static readonly Faker<Person> PersonFaker;
        private static readonly Faker<Post> PostFaker;
#if NET
        private static readonly Randomizer Random = new();
#else
        private static readonly Randomizer Random = new Randomizer();
#endif
        private static readonly Faker<User> UserFaker;
        private static readonly Faker<UserCredential> UserCredentialFaker;

        public static Blog Blog => BlogFaker.Generate();

        public static IList<Blog> Blogs => BlogFaker.Generate(Random.Int(1, BlogMaxCount));

        public static Credential Credential => CredentialFaker.Generate();

        public static IList<Credential> Credentials => CredentialFaker.Generate(Random.Int(1, CredentialMaxCount));

        public static CredentialIssuer CredentialIssuer => CredentialIssuerFaker.Generate();

        public static IList<CredentialIssuer> CredentialIssuers =>
            CredentialIssuerFaker.Generate(Random.Int(1, CredentialIssuerMaxCount));

        public static Person Person => PersonFaker.Generate();

        public static IList<Person> Persons => PersonFaker.Generate(Random.Int(1, PersonMaxCount));

        public static Post Post => PostFaker.Generate();

        public static IList<Post> Posts => PostFaker.Generate(Random.Int(1, PostMaxCount));

        public static User User => UserFaker.Generate();

        public static IList<User> Users => UserFaker.Generate(Random.Int(1, UserMaxCount));

        public static UserCredential UserCredential => UserCredentialFaker.Generate();

        public static IList<UserCredential> UserCredentials =>
            UserCredentialFaker.Generate(Random.Int(1, UserCredentialMaxCount));

        static DataFactory()
        {
            BlogFaker = new Faker<Blog>()
                .UseSeed(BlogSeed)
                .RuleForBaseModel()
                .RuleFor(b => b.Name, f => f.Lorem.Word())
                .RuleFor(b => b.Rating, f => f.Random.Int(1, 10))
                .RuleFor(b => b.Url, f => f.Internet.Url());

            CredentialIssuerFaker = new Faker<CredentialIssuer>()
                .UseSeed(CredentialIssuerSeed)
                .RuleForBaseModel()
                .RuleFor(c => c.Name, f => f.Company.CompanyName());

            CredentialFaker = new Faker<Credential>()
                .UseSeed(CredentialSeed)
                .RuleForBaseModel()
                .RuleFor(c => c.Name, f => f.Commerce.ProductName())
                .RuleFor(c => c.Issuers, _ => CredentialIssuerFaker.Generate(Random.Int(1, CredentialIssuerMaxCount)));

            PersonFaker = new Faker<Person>()
                .UseSeed(PersonSeed)
                .RuleForBaseModel()
                .RuleFor(p => p.Name, f => f.Person.FullName);

            PostFaker = new Faker<Post>()
                .UseSeed(PostSeed)
                .RuleForBaseModel()
                .RuleFor(p => p.Content, f => f.Lorem.Paragraph())
                .RuleFor(p => p.Title, f => f.Lorem.Sentence());

            UserCredentialFaker = new Faker<UserCredential>()
                .UseSeed(UserCredentialSeed)
                .RuleForBaseModel()
                .RuleFor(u => u.Status, f => f.Internet.Color())
                .RuleFor(u => u.Credentials, _ => CredentialFaker.Generate(Random.Int(1, CredentialMaxCount)));

            UserFaker = new Faker<User>()
                .UseSeed(UserSeed)
                .RuleForBaseModel()
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(
                    u => u.UserCredentials,
                    _ => UserCredentialFaker.Generate(Random.Int(1, UserCredentialMaxCount)));
        }

        public static Person CreatePerson()
        {
            Person person = PersonFaker.Generate();
            Blog blog = BlogFaker.Generate();
            List<Post> posts = PostFaker.Generate(Random.Int(1, Random.Int(1, PostMaxCount)));

            person.OwnedBlog = blog;
            //person.Posts = posts;
            blog.Posts = posts;

            return person;
        }
    }
}