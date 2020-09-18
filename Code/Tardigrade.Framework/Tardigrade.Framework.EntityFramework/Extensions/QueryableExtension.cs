using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tardigrade.Framework.EntityFramework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for IQueryable.
    /// </summary>
    public static class QueryableExtension
    {
        /// <summary>
        /// Query a specific class of a class hierarchy without applying the query to all the underlying derived classes.
        /// <a href="https://docs.microsoft.com/en-us/archive/blogs/alexj/tip-35-how-to-write-oftypeonlytentity">Tip 35 – How to write OfTypeOnly()</a>
        /// <a href="https://stackoverflow.com/questions/2056092/retrieve-only-base-class-from-entity-framework">Retrieve only base class from Entity Framework</a>
        /// </summary>
        /// <typeparam name="TBaseEntity">Type of the base class.</typeparam>
        /// <typeparam name="TDerivedEntity">Type of the derived class.</typeparam>
        /// <param name="query">IQueryable this extension is applied to.</param>
        /// <returns>An IQueryable for a specific class, ignoring all underlying derived classes.</returns>
        public static IQueryable<TDerivedEntity> OfTypeOnly<TBaseEntity, TDerivedEntity>(
            this IQueryable<TBaseEntity> query)
            where TDerivedEntity : TBaseEntity
        {
            // Look just for immediate subclasses as that will be enough to remove any generations below.
            IEnumerable<Type> subTypes = typeof(TDerivedEntity).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(TDerivedEntity)))
                .ToList();

            if (!subTypes.Any())
            {
                return query.OfType<TDerivedEntity>();
            }

            // Start with a parameter of the type of the query.
            ParameterExpression parameter = Expression.Parameter(typeof(TDerivedEntity));

            // Build up an expression excluding all the sub-types.
            Expression removeAllSubTypes = null;

            foreach (Type subType in subTypes)
            {
                // For each sub-type, add a clause to make sure that the parameter is not of this type.
                UnaryExpression removeThisSubType = Expression.Not(Expression.TypeIs(parameter, subType));

                // Merge with the previous expressions.
                if (removeAllSubTypes == null)
                {
                    removeAllSubTypes = removeThisSubType;
                }
                else
                {
                    removeAllSubTypes = Expression.AndAlso(removeAllSubTypes, removeThisSubType);
                }
            }

            // Convert to a lambda (actually pass the parameter in).
            LambdaExpression removeAllSubTypesLambda = Expression.Lambda(removeAllSubTypes, parameter);

            // Filter the query.
            return query.OfType<TDerivedEntity>()
                .Where(removeAllSubTypesLambda as Expression<Func<TDerivedEntity, bool>>);
        }
    }
}