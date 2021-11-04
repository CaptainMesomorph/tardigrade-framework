using AutoMapper;
using System.Collections.Generic;

namespace Tardigrade.Framework.Converters
{
    /// <summary>
    /// Converts a source member value to a collection of destination member values.
    /// </summary>
    /// <typeparam name="TSource">Source type.</typeparam>
    /// <typeparam name="TDestination">Destination type.</typeparam>
    public class SingleToListValueConverter<TSource, TDestination>
        : IValueConverter<TSource, ICollection<TDestination>>
    {
        /// <inheritdoc />
        public ICollection<TDestination> Convert(TSource sourceMember, ResolutionContext context)
        {
            var destination = context.Mapper.Map<TDestination>(sourceMember);

            return new List<TDestination> { destination };
        }
    }
}