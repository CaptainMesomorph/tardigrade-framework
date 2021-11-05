using Bogus;
using System;
using Tardigrade.Framework.RestSharp.Tests.Models;

namespace Tardigrade.Framework.RestSharp.Tests.Extensions
{
    public static class FakerExtension
    {
        public static Faker<User> RuleForUser(this Faker<User> faker)
        {
            if (faker == null) throw new ArgumentNullException(nameof(faker));

            return faker
                .RuleFor(u => u.DateOfBirth, f => f.Date.Past().ToString("dd/MM/yyyy hh:mm tt"))
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.MiddleNames, f => f.Name.FullName());
        }
    }
}