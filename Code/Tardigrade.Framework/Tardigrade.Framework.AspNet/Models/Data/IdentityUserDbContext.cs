using Microsoft.AspNet.Identity.EntityFramework;
using Tardigrade.Framework.AspNet.Models.Identity;

namespace Tardigrade.Framework.AspNet.Models.Data
{
    /// <summary>
    /// Entity Framework database context for use with ASP.NET Identity.
    /// </summary>
    public class IdentityUserDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="nameOrConnectionString">Name of the database connection or database connection string.</param>
        public IdentityUserDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }
    }
}