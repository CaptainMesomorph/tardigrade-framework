using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using Tardigrade.Framework.Patterns.Command;

namespace Tardigrade.Framework.SimpleInjector
{
    /// <summary>
    /// Proxy class (based on Command Handlers) for managing a Scoped lifetime for an object instance.
    /// <a href="https://simpleinjector.readthedocs.io/en/latest/aop.html">Aspect-Oriented Programming</a>
    /// </summary>
    /// <typeparam name="TCommand">Type of (data) object associated with the Command Handler.</typeparam>
    public class AsyncScopedProxy<TCommand> : ICommandHandler<TCommand>
    {
        private readonly Container container;
        private readonly Func<ICommandHandler<TCommand>> decorateeFactory;

        /// <summary>
        /// Create an instance of this proxy class.
        /// </summary>
        /// <param name="container">SimpleInjector DI container.</param>
        /// <param name="decorateeFactory">Decoratee factory.</param>
        public AsyncScopedProxy(Container container, Func<ICommandHandler<TCommand>> decorateeFactory)
        {
            this.container = container;
            this.decorateeFactory = decorateeFactory;
        }

        /// <summary>
        /// <see cref="ICommandHandler{TCommand}.Handle(TCommand)"/>
        /// </summary>
        public void Handle(TCommand command)
        {
            // Start a new scope.
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                // Create the decorateeFactory within the scope.
                ICommandHandler<TCommand> handler = decorateeFactory.Invoke();
                handler.Handle(command);
            }
        }
    }
}