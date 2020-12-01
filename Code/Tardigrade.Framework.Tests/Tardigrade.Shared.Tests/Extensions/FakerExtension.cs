using Bogus;
using System;
using Tardigrade.Shared.Tests.Models;

namespace Tardigrade.Shared.Tests.Extensions
{
    public static class FakerExtension
    {
        public static Faker<TModel> RuleForBaseModel<TModel>(this Faker<TModel> faker) where TModel : BaseModel
        {
            if (faker == null) throw new ArgumentNullException(nameof(faker));

            return faker
                .RuleFor(u => u.CreatedBy, f => f.Random.Guid().ToString())
                .RuleFor(u => u.CreatedDate, f => f.Date.Past())
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.IsDeleted, f => f.Random.Bool())
                .RuleFor(u => u.ModifiedBy, f => f.Random.Guid().ToString())
                .RuleFor(u => u.ModifiedDate, f => f.Date.Past());
        }
    }
}