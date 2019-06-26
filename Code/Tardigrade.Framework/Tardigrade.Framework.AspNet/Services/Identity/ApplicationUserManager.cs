using Microsoft.AspNet.Identity;
using Tardigrade.Framework.AspNet.Models.Identity;

namespace Tardigrade.Framework.AspNet.Services.Identity
{
    /// <summary>
    /// ASP.NET Identity user manager configured for this application.
    /// </summary>
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        /// <summary>
        /// <see cref="UserManager{TUser}.UserManager(IUserStore{TUser})"/>
        /// </summary>
        public ApplicationUserManager(IUserStore<ApplicationUser> store) : base(store)
        {
        }
    }
}