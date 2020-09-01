namespace Tardigrade.Framework.Models.Domain
{
    /// <summary>
    /// Interface for defining a domain model with a unique identifier.
    /// </summary>
    /// <typeparam name="PK">Type of the unique identifier.</typeparam>
    public interface IHasUniqueIdentifier<PK>
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        PK Id { get; set; }
    }
}