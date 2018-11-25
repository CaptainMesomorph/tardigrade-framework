﻿namespace Tardigrade.Framework.Patterns.Command
{
    /// <summary>
    /// Handler that represents an atomic business operation.
    /// <see cref="!:https://cuttingedge.it/blogs/steven/pivot/entry.php?id=91">Meanwhile... on the command side of my architecture</see>
    /// </summary>
    /// <typeparam name="TCommand">Type of (data) object associated with the handler.</typeparam>
    public interface ICommandHandler<TCommand>
    {
        /// <summary>
        /// Method to perform the business operation.
        /// </summary>
        /// <param name="command">The (data) object associated with the business operation.</param>
        void Handle(TCommand command);
    }
}