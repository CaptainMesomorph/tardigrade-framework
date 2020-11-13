using System;
using System.Linq;

namespace Tardigrade.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the IQueryable interface.
    /// </summary>
    public static class QueryableExtension
    {
        /// <summary>
        /// Apply a sort condition based upon a string of the format: <![CDATA[[<property>:ASC|DESC,]+]]>. For example,
        /// "lastname:desc,firstname:asc". The order specified (ASC or DESC) is case insensitive.
        /// <a href="https://www.itorian.com/2015/12/sorting-in-webapi-generic-way-to-apply.html">Sorting in WebAPI - a generic way to apply sorting</a>
        /// </summary>
        /// <typeparam name="T">Type associated with the IQueryable.</typeparam>
        /// <param name="source">IQueryable this extension is applied to.</param>
        /// <param name="sortBy">String containing the sort condition.</param>
        /// <returns>An IQueryable with the sort condition applied.</returns>
        /// <exception cref="ArgumentException">Format of sortBy is invalid.</exception>
        /// <exception cref="ArgumentNullException">source is null, or sortBy is null or empty.</exception>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string sortBy)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrWhiteSpace(sortBy)) throw new ArgumentNullException(nameof(sortBy));

            string[] sortItems = sortBy.Trim().Split(',');
            string sortExpression = string.Empty;

            foreach (string sortItem in sortItems)
            {
                string[] condition = sortItem.Trim().Split(':');

                if (condition.Length != 2)
                {
                    throw new ArgumentException("Format of sort condition invalid; property/order pair incorrectly defined.", nameof(sortBy));
                }

                string property = condition[0].Trim();
                string order = condition[1].Trim();

                if (string.IsNullOrWhiteSpace(property))
                {
                    throw new ArgumentException("Format of sort condition invalid; no property specified.", nameof(sortBy));
                }

                if (!"asc".Equals(order, StringComparison.InvariantCultureIgnoreCase) &&
                    !"desc".Equals(order, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ArgumentException("Format of sort condition invalid; order must be ASC or DESC.", nameof(sortBy));
                }

                sortExpression += $"{condition[0].Trim()} {condition[1].Trim()},";
            }

            return source.OrderBy(sortExpression.Remove(sortExpression.Length - 1));
        }
    }
}