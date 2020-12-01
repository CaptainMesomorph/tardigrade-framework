using Bogus;
using System.Collections.Generic;
using Tardigrade.Shared.Tests.Extensions;
using Tardigrade.Shared.Tests.Models;

namespace Tardigrade.Shared.Tests
{
    public class DataFactory
    {
        private static readonly Faker<Credential> FakeCredentials = new Faker<Credential>()
            .RuleForBaseModel()
            .RuleFor(u => u.Name, f => f.Commerce.ProductName())
            .RuleFor(u => u.Issuers, f => FakeCredentialIssuers.Generate(5));

        private static readonly Faker<CredentialIssuer> FakeCredentialIssuers = new Faker<CredentialIssuer>()
            .RuleForBaseModel()
            .RuleFor(u => u.Name, f => f.Company.CompanyName());

        private static readonly Faker<User> FakeUsers = new Faker<User>()
            .RuleForBaseModel()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .RuleFor(u => u.UserCredentials, f => FakeUserCredentials.Generate(5));

        private static readonly Faker<UserCredential> FakeUserCredentials = new Faker<UserCredential>()
            .RuleForBaseModel()
            .RuleFor(u => u.Status, f => f.Internet.Color())
            .RuleFor(u => u.Credentials, f => FakeCredentials.Generate(5));

        public static Credential CreateCredential() => FakeCredentials.Generate();

        public static ICollection<Credential> CreateCredentials(uint count) => FakeCredentials.Generate((int)count);

        public static CredentialIssuer CreateCredentialIssuer() => FakeCredentialIssuers.Generate();

        public static ICollection<CredentialIssuer> CreateCredentialIssuers(uint count) =>
            FakeCredentialIssuers.Generate((int)count);

        public static User CreateUser() => FakeUsers.Generate();

        public static ICollection<User> CreateUsers(uint count) => FakeUsers.Generate((int)count);

        public static UserCredential CreateUserCredential() => FakeUserCredentials.Generate();

        public static ICollection<UserCredential> CreateUserCredentials(uint count) =>
            FakeUserCredentials.Generate((int)count);
    }
}