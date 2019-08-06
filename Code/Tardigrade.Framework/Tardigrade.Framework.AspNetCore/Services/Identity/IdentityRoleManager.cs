using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tardigrade.Framework.AspNetCore.Models.Identity;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Services.Identity;

namespace Tardigrade.Framework.AspNetCore.Services.Identity
{
    /// <summary>
    /// <see cref="IIdentityRoleManager{T}"/>
    /// </summary>
    public class IdentityRoleManager : IIdentityRoleManager<ApplicationRole>, IDisposable
    {
        private bool disposedValue = false; // To detect redundant calls
        private RoleManager<ApplicationRole> roleManager;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="roleManager">Service for managing user roles.</param>
        /// <exception cref="ArgumentNullException">roleManager is null.</exception>
        public IdentityRoleManager(RoleManager<ApplicationRole> roleManager)
        {
            this.roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        /// <summary>
        /// <see cref="IIdentityRoleManager{T}.CreateAsync(T)"/>
        /// </summary>
        public async Task<ApplicationRole> CreateAsync(ApplicationRole role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            IdentityResult result = await roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                IEnumerable<Framework.Models.Errors.IdentityError> errors =
                    result.Errors.Select(e => new Framework.Models.Errors.IdentityError(e.Code, e.Description));

                throw new IdentityException($"Create role failed; unable to create role {role.Id}.", errors);
            }

            return role;
        }

        /// <summary>
        /// <see cref="IIdentityRoleManager{T}.DeleteAsync(T)"/>
        /// </summary>
        public async Task DeleteAsync(ApplicationRole role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            IdentityResult result = await roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                IEnumerable<Framework.Models.Errors.IdentityError> errors =
                    result.Errors.Select(e => new Framework.Models.Errors.IdentityError(e.Code, e.Description));

                throw new IdentityException($"Delete role failed; unable to delete role {role.Id}.", errors);
            }
        }

        /// <summary>
        /// <see cref="IDisposable.Dispose()"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Cleanly dispose this service.
        /// </summary>
        /// <param name="disposing">True to dispose; false otherwise.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (roleManager != null)
                    {
                        roleManager.Dispose();
                        roleManager = null;
                    }
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// <see cref="IIdentityRoleManager{T}.ExistsAsync(string)"/>
        /// </summary>
        public async Task<bool> ExistsAsync(string roleName)
        {
            if (roleName == null) throw new ArgumentNullException(nameof(roleName));

            bool exists = await roleManager.RoleExistsAsync(roleName);

            return exists;
        }

        /// <summary>
        /// <see cref="IIdentityRoleManager{T}.RetrieveAsync(string)"/>
        /// </summary>
        public async Task<ApplicationRole> RetrieveAsync(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId)) throw new ArgumentNullException(nameof(roleId));

            return await roleManager.FindByIdAsync(roleId);
        }

        /// <summary>
        /// <see cref="IIdentityRoleManager{T}.RetrieveByNameAsync(string)"/>
        /// </summary>
        public async Task<ApplicationRole> RetrieveByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

            return await roleManager.FindByNameAsync(roleName);
        }

        /// <summary>
        /// <see cref="IIdentityRoleManager{T}.UpdateAsync(T)"/>
        /// </summary>
        public async Task UpdateAsync(ApplicationRole role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            IdentityResult result = await roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                IEnumerable<Framework.Models.Errors.IdentityError> errors =
                    result.Errors.Select(e => new Framework.Models.Errors.IdentityError(e.Code, e.Description));

                throw new IdentityException($"Update role failed; unable to update role {role.Id}.", errors);
            }
        }
    }
}