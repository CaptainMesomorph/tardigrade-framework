using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tardigrade.Framework.AspNetCore.Models.Identity;

namespace Tardigrade.Framework.AspNetCore.Services.Identity
{
    /// <summary>
    /// ASP.NET Core Identity sign-in manager configured for this application.
    /// </summary>
    public class ApplicationSignInManager : SignInManager<ApplicationUser>
    {
        /// <summary>
        /// <see cref="SignInManager{TUser}.SignInManager(UserManager{TUser}, IHttpContextAccessor, IUserClaimsPrincipalFactory{TUser}, IOptions{IdentityOptions}, ILogger{SignInManager{TUser}}, IAuthenticationSchemeProvider, IUserConfirmation{TUser})"/>
        /// </summary>
        public ApplicationSignInManager(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<ApplicationUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }
    }
}