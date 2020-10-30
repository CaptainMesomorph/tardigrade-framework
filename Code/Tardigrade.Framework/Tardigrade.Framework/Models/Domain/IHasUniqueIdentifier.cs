namespace Tardigrade.Framework.Models.Domain
{
    /// <summary>
    /// Interface for defining a domain model with a unique identifier.
    /// </summary>
    /// <typeparam name="TKey">Type of the unique identifier.</typeparam>
    public interface IHasUniqueIdentifier<TKey>
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        TKey Id { get; set; }
    }
}