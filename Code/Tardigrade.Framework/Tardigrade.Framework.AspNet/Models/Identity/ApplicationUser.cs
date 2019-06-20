using Microsoft.AspNet.Identity.EntityFramework;
using Tardigrade.Framework.Models.Identity;

namespace Tardigrade.Framework.AspNet.Models.Identity
{
    /// <summary>
    /// Represents an application user based upon an ASP.NET Identity user.
    /// </summary>
    public class ApplicationUser : IdentityUser, IApplicationUser
    {
    }
}