using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Tardigrade.Framework.AspNetCore.Models.Identity;

namespace Tardigrade.Framework.AspNetCore.Services.Identity
{
    /// <summary>
    /// ASP.NET Identity role manager configured for this application.
    /// </summary>
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        /// <summary>
        /// <see cref="RoleManager{TRole}.RoleManager(IRoleStore{TRole}, IEnumerable{IRoleValidator{TRole}}, ILookupNormalizer, IdentityErrorDescriber, ILogger{RoleManager{TRole}})"/>
        /// </summary>
        public ApplicationRoleManager(
            IRoleStore<ApplicationRole> store,
            IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager<ApplicationRole>> logger)
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}