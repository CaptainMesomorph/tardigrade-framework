using Microsoft.AspNet.Identity.EntityFramework;
using Tardigrade.Framework.AspNet.Models.Identity;

namespace Tardigrade.Framework.EntityFramework.Data
{
    /// <summary>
    /// Entity Framework Core database context for the application.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="nameOrConnectionString">Name of the database connection or database connection string.</param>
        public ApplicationDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }
    }
}