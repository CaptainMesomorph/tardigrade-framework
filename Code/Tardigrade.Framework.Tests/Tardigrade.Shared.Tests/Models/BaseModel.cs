using System;
using Tardigrade.Framework.Models.Domain;

namespace Tardigrade.Shared.Tests.Models
{
    /// <summary>
    /// Base class of common properties.
    /// </summary>
    public abstract class BaseModel : IAuditable<string>, IHasUniqueIdentifier<Guid>, ISoftDeletable
    {
        /// <summary>
        /// <see cref="IAuditable{T}.CreatedBy"/>
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// <see cref="IAuditable{T}.CreatedDate"/>
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// <see cref="ISoftDeletable.IsDeleted"/>
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// <see cref="IAuditable{T}.ModifiedBy"/>
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// <see cref="IAuditable{T}.ModifiedDate"/>
        /// </summary>
        public DateTime ModifiedDate { get; set; }
    }
}