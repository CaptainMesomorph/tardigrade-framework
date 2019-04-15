using System.Collections.Generic;
using System.Linq;

namespace Tardigrade.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for collections.
    /// </summary>
    public static class CollectionExtension
    {
        /// <summary>
        /// If the specified collection is null, return an empty collection. If not, return the collection as is.
        /// Example usage:
        /// <code>foreach (UserProfile userProfile in userProfiles.OrEmptyIfNull())</code>
        /// <a href="https://stackoverflow.com/questions/11734380/check-for-null-in-foreach-loop">Check for null in foreach loop</a>
        /// </summary>
        /// <typeparam name="T">Type associated with the collection.</typeparam>
        /// <param name="source">The collection to check against null.</param>
        /// <returns>The collection itself if not null; an empty collection otherwise.</returns>
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}