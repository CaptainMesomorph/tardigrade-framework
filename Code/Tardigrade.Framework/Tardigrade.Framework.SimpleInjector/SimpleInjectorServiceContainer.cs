using SimpleInjector;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using Tardigrade.Framework.Patterns.DependencyInjection;

namespace Tardigrade.Framework.SimpleInjector
{
    /// <summary>
    /// Service container based the SimpleInjector Dependency Injection framework.
    /// </summary>
    public abstract class SimpleInjectorServiceContainer : IServiceContainer
    {
        // TODO: Replace with ILogger.
        //private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// SimpleInjector services container.
        /// </summary>
        protected static Container Container { get; private set; }

        /// <summary>
        /// Create an instance of this service container.
        /// </summary>
        /// <param name="container">Existing service container to use. If null, a new service container instance is created.</param>
        /// <param name="verifyImmediately">If true, verify services during instantiation. If false, defer verification with a call to Verify().</param>
        public SimpleInjectorServiceContainer(Container container = null, bool verifyImmediately = true)
        {
            // Create the services container.
            if (container == null)
            {
                Container = new Container();
            }
            else
            {
                Container = container;
            }

            try
            {
                // Register types.
                ConfigureServices(Container);
            }
            catch (Exception e)
            {
                string errorMessage = ProcessLoadException(e);

                if (errorMessage == null)
                {
                    //if (log.IsErrorEnabled) log.Error(e, e.GetBaseException().Message);
                }
                else
                {
                    //if (log.IsErrorEnabled) log.Error(e, errorMessage);
                }

                throw;
            }

            if (verifyImmediately)
            {
                Container.Verify();
            }
        }

        /// <summary>
        /// <see cref="IServiceContainer.GetService{T}"/>
        /// </summary>
        public T GetService<T>() where T : class
        {
            return Container.GetInstance<T>();
        }

        /// <summary>
        /// Add services to the service container.
        /// </summary>
        /// <param name="container">Services container.</param>
        public abstract void ConfigureServices(Container container);

        /// <summary>
        /// Generate an informative message for errors associated with type (class) loading.
        /// </summary>
        /// <param name="exception">Exception to process.</param>
        /// <returns>Informative message if the error is associated with type loading; null otherwise.</returns>
        private static string ProcessLoadException(Exception exception)
        {
            string message = null;

            if (exception is ReflectionTypeLoadException)
            {
                ReflectionTypeLoadException typeLoadException = exception as ReflectionTypeLoadException;
                StringBuilder stringBuilder = new StringBuilder();

                foreach (Exception loaderException in typeLoadException.LoaderExceptions)
                {
                    stringBuilder.AppendLine(loaderException.Message);

                    if (loaderException is FileNotFoundException fileNotFoundException)
                    {
                        if (!string.IsNullOrEmpty(fileNotFoundException.FusionLog))
                        {
                            stringBuilder.AppendLine($"Fusion Log: {fileNotFoundException.FusionLog}");
                        }
                    }

                    stringBuilder.AppendLine();
                }

                message = stringBuilder.ToString();
            }
            else if (exception.InnerException != null)
            {
                message = ProcessLoadException(exception.InnerException);
            }

            return message;
        }

        /// <summary>
        /// Verifies the services in this service container.
        /// </summary>
        /// <exception cref="InvalidOperationException">Registration of instances was invalid.</exception>
        public void Verify()
        {
            Container.Verify();
        }
    }
}