using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Tardigrade.Framework.AspNet.Models.Identity;

namespace Tardigrade.Framework.AspNet.Services.Identity
{
    /// <summary>
    /// ASP.NET Identity sign-in manager configured for this application.
    /// </summary>
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        /// <summary>
        /// <see cref="SignInManager{TUser, TKey}.SignInManager(UserManager{TUser, TKey}, IAuthenticationManager)"/>
        /// </summary>
        public ApplicationSignInManager(
            UserManager<ApplicationUser> userManager,
            IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
    }
}