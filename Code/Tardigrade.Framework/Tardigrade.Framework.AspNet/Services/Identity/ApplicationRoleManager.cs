using Microsoft.AspNet.Identity;
using Tardigrade.Framework.AspNet.Models.Identity;

namespace Tardigrade.Framework.AspNet.Services.Identity
{
    /// <summary>
    /// ASP.NET Identity role manager configured for this application.
    /// </summary>
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        /// <summary>
        /// <see cref="RoleManager{TRole}.RoleManager(IRoleStore{TRole, string})"/>
        /// </summary>
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> store) : base(store)
        {
        }
    }
}