using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tardigrade.Framework.Models.Domain;

namespace Tardigrade.Framework.EntityFrameworkCore.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the ModelBuilder class.
    /// </summary>
    public static class ModelBuilderExtension
    {
        /// <summary>
        /// Create a query filter to ignore soft deleted entities (i.e. ISoftDeletable.IsDeleted = true).
        /// <a href="https://www.meziantou.net/entity-framework-core-soft-delete-using-query-filters.htm">Entity Framework Core: Soft Delete using Query Filters</a>
        /// <a href="https://github.com/dotnet/efcore/issues/18084">HasQueryFilter not working from Entity Framework Core 3.0</a>
        /// </summary>
        /// <param name="modelBuilder">The builder used to manage the model for the database context.</param>
        public static void FilterSoftDeleted(this ModelBuilder modelBuilder)
        {
            IEnumerable<IMutableEntityType> entityTypes =
                modelBuilder.Model.GetEntityTypes().Where(e => typeof(ISoftDeletable).IsAssignableFrom(e.ClrType));

            // Create a query filter for all types that implement the ISoftDeletable interface.
            foreach (IMutableEntityType entityType in entityTypes)
            {
                // EF.Property<bool>(entity, "IsDeleted")
                ParameterExpression parameter = Expression.Parameter(entityType.ClrType);
                Expression left = Expression.Call(
                    typeof(EF),
                    nameof(EF.Property),
                    new[] { typeof(bool) },
                    parameter,
                    Expression.Constant("IsDeleted"));

                // EF.Property<bool>(entity, "IsDeleted") == false
                Expression right = Expression.Constant(false);
                BinaryExpression body = Expression.Equal(left, right);

                // entity => EF.Property<bool>(entity, "IsDeleted") == false
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, parameter));
            }
        }
    }
}