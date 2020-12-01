using System;

namespace Tardigrade.Framework.Models.Domain
{
    /// <summary>
    /// Interface that specifies a domain model which has audit information associated with it.
    /// </summary>
    public interface IAuditable<T>
    {
        /// <summary>
        /// Identifier of the user who created a new instance of the model.
        /// </summary>
        T CreatedBy { get; set; }

        /// <summary>
        /// Date a new instance of the model was created.
        /// </summary>
        DateTime CreatedDate { get; set; }

        /// <summary>
        /// Identifier of the user who modified an existing instance of the model.
        /// </summary>
        T ModifiedBy { get; set; }

        /// <summary>
        /// Date an existing instance of the model was modified.
        /// </summary>
        DateTime ModifiedDate { get; set; }
    }
}