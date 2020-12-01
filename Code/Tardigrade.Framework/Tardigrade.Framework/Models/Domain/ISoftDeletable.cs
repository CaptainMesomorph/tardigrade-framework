namespace Tardigrade.Framework.Models.Domain
{
    /// <summary>
    /// Interface for defining whether a domain model can be soft deleted.
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        /// Flag indicating whether the domain model has been soft deleted.
        /// </summary>
        bool IsDeleted { get; set; }
    }
}