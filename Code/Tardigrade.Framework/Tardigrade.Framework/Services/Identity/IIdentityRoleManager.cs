using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tardigrade.Framework.Services.Identity
{
    /// <summary>
    /// Interface that defines operations associated with user roles.
    /// </summary>
    /// <typeparam name="TRole">Type associated with the user role definition.</typeparam>
    public interface IIdentityRoleManager<TRole>
    {
        /// <summary>
        /// Create a role.
        /// </summary>
        /// <param name="role">Role to create.</param>
        /// <returns>Role created (including allocated unique identifier).</returns>
        /// <exception cref="System.ArgumentNullException">role is null.</exception>
        /// <exception cref="Exceptions.IdentityException">Error creating the role.</exception>
        Task<TRole> CreateAsync(TRole role);

        /// <summary>
        /// Delete a role.
        /// </summary>
        /// <param name="role">Role to delete.</param>
        /// <returns>Task object representing the asynchronous operation result.</returns>
        /// <exception cref="System.ArgumentNullException">role is null.</exception>
        /// <exception cref="Exceptions.IdentityException">Error deleting the role.</exception>
        Task DeleteAsync(TRole role);

        /// <summary>
        /// Check for existence of a role by name.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>True if the role exists; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">roleName is null.</exception>
        Task<bool> ExistsAsync(string roleName);

        /// <summary>
        /// Retrieve all the roles available.
        /// </summary>
        /// <returns>All the roles if they exist; empty collection otherwise.</returns>
        Task<IList<TRole>> RetrieveAsync();

        /// <summary>
        /// Retrieve a role with the specified unique identifier.
        /// </summary>
        /// <param name="roleId">Unique identifier for the role.</param>
        /// <returns>Role matching the specified unique identifier if it exists; null otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">roleId is null or empty.</exception>
        Task<TRole> RetrieveAsync(string roleId);

        /// <summary>
        /// Retrieve a role user by role name.
        /// </summary>
        /// <param name="roleName">Role name to search for.</param>
        /// <returns>Role matching the specified role name if found; null otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">roleName is null or empty.</exception>
        Task<TRole> RetrieveByNameAsync(string roleName);

        /// <summary>
        /// Update a role.
        /// </summary>
        /// <param name="role">Role to update.</param>
        /// <returns>Task object representing the asynchronous operation result.</returns>
        /// <exception cref="System.ArgumentNullException">role is null.</exception>
        /// <exception cref="Exceptions.IdentityException">Error updating the role.</exception>
        Task UpdateAsync(TRole role);
    }
}