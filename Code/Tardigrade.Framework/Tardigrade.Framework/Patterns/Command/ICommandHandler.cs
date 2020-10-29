namespace Tardigrade.Framework.Patterns.Command
{
    /// <summary>
    /// Handler that represents an atomic business operation.
    /// <a href="https://cuttingedge.it/blogs/steven/pivot/entry.php?id=91">Meanwhile... on the command side of my architecture</a>
    /// </summary>
    /// <typeparam name="TCommand">Type of (data) object associated with the handler.</typeparam>
    public interface ICommandHandler<in TCommand>
    {
        /// <summary>
        /// Method to perform the business operation.
        /// </summary>
        /// <param name="command">The (data) object associated with the business operation.</param>
        void Handle(TCommand command);
    }
}