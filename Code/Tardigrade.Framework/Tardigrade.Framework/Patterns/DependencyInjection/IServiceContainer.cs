namespace Tardigrade.Framework.Patterns.DependencyInjection
{
    /// <summary>
    /// Interface class that acts as a container for instantiated services.
    /// </summary>
    public interface IServiceContainer
    {
        /// <summary>
        /// Retrieve the instantiated service of a particular type.
        /// </summary>
        /// <typeparam name="T">Type of the service to retrieve.</typeparam>
        /// <returns>Instance of the service.</returns>
        T GetService<T>() where T : class;
    }
}