using Microsoft.AspNetCore.Identity;
using Tardigrade.Framework.Models.Identity;

namespace Tardigrade.Framework.AspNetCore.Models.Identity
{
    /// <summary>
    /// Represents an application user based upon an ASP.NET Core Identity user.
    /// </summary>
    public class ApplicationUser : IdentityUser, IApplicationUser
    {
    }
}