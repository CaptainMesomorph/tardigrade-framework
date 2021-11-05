using Bogus;
using System.Collections.Generic;
using Tardigrade.Framework.RestSharp.Tests.Extensions;
using Tardigrade.Framework.RestSharp.Tests.Models;

namespace Tardigrade.Framework.RestSharp.Tests.Factories
{
    public static class DataFactory
    {
        private const int UserSeed = 56789;

        private static readonly Randomizer Random = new();
        private static readonly Faker<User> RandomUserFaker;
        private static readonly Faker<User> SameUserFaker;

        public static IList<User> Random10Users => RandomUserFaker.Generate(Random.Int(1, 10));

        public static User RandomUser => RandomUserFaker.Generate();

        public static User SameUser => SameUserFaker.Generate();

        static DataFactory()
        {
            RandomUserFaker = new Faker<User>()
                .RuleForUser();

            SameUserFaker = new Faker<User>()
                .UseSeed(UserSeed)
                .RuleForUser();
        }
    }
}