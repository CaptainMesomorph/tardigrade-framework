using AutoMapper;
using System.Collections.Generic;

namespace Tardigrade.Framework.Converters
{
    /// <summary>
    /// Converts the source type to a collection of destination types.
    /// <a href="https://gist.github.com/frankhale/fe9159b4c7df9bf5d8cfd7656cd816f1">Map single object to list of objects in AutoMapper</a>
    /// </summary>
    /// <typeparam name="TSource">Source type.</typeparam>
    /// <typeparam name="TDestination">Destination type.</typeparam>
    public class SingleToListTypeConverter<TSource, TDestination> : ITypeConverter<TSource, IEnumerable<TDestination>>
    {
        /// <inheritdoc />
        public IEnumerable<TDestination> Convert(
            TSource source,
            IEnumerable<TDestination> destination,
            ResolutionContext context)
        {
            var dest = context.Mapper.Map<TDestination>(source);

            return new List<TDestination>() { dest };
        }
    }
}