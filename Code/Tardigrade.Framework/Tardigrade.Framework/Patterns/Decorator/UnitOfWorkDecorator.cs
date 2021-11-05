using System;
using Tardigrade.Framework.Patterns.Command;
using Tardigrade.Framework.Patterns.UnitOfWork;

namespace Tardigrade.Framework.Patterns.Decorator
{
    /// <summary>
    /// Decorator class (based on Command Handlers) for managing the Unit of Work pattern.
    /// <a href="https://simpleinjector.readthedocs.io/en/latest/aop.html">Aspect-Oriented Programming</a>
    /// <a href="https://cuttingedge.it/blogs/steven/pivot/entry.php?id=91">Meanwhile... on the command side of my architecture</a>
    /// </summary>
    /// <typeparam name="TCommand">Type of (data) object associated with the Command Handler.</typeparam>
    public class UnitOfWorkDecorator<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> _decoratedHandler;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Create an instance of this Decorator.
        /// </summary>
        /// <param name="decoratedHandler">Command Handler that is being decorated.</param>
        /// <param name="unitOfWork">Unit of Work manager.</param>
        public UnitOfWorkDecorator(ICommandHandler<TCommand> decoratedHandler, IUnitOfWork unitOfWork)
        {
            _decoratedHandler = decoratedHandler;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Perform the business operation using the Unit of Work pattern.
        /// </summary>
        /// <param name="command">The (data) object associated with the business operation.</param>
        public void Handle(TCommand command)
        {
            _unitOfWork.Begin();

            try
            {
                _decoratedHandler.Handle(command);
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
    }
}