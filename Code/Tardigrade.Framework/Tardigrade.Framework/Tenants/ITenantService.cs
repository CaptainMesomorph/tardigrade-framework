using System.Threading.Tasks;

namespace Tardigrade.Framework.Tenants
{
    /// <summary>
    /// Service interface for operations associated with managing the current tenant.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITenantService<T>
    {
        /// <summary>
        /// Delete the current tenant.
        /// </summary>
        void Delete();

        /// <summary>
        /// Delete the current tenant.
        /// </summary>
        /// <returns>Result associated with the asynchronous operation.</returns>
        Task DeleteAsync();

        /// <summary>
        /// Check whether the current tenant has been defined.
        /// </summary>
        /// <returns>True if the current tenant is defined; false otherwise.</returns>
        bool Exists();

        /// <summary>
        /// Check whether the current tenant has been defined.
        /// </summary>
        /// <returns>True if the current tenant is defined; false otherwise.</returns>
        Task<bool> ExistsAsync();

        /// <summary>
        /// Retrieve the current tenant.
        /// </summary>
        /// <returns>Current tenant if it exists; null otherwise.</returns>
        T Retrieve();

        /// <summary>
        /// Retrieve the current tenant.
        /// </summary>
        /// <returns>Current tenant if it exists; null otherwise.</returns>
        Task<T> RetrieveAsync();

        /// <summary>
        /// Set the current tenant.
        /// </summary>
        /// <returns>Result associated with the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">tenant is null or empty.</exception>
        Task SaveAsync(T tenant);

        /// <summary>
        /// Set the current tenant.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">tenant is null or empty.</exception>
        void Save(T tenant);
    }
}