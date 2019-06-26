using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tardigrade.Framework.AspNetCore.Models.Identity;

namespace Tardigrade.Framework.EntityFrameworkCore.Data
{
    /// <summary>
    /// Entity Framework Core database context for use with ASP.NET Core Identity.
    /// </summary>
    public class IdentityUserDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="options">Options associated with the database context.</param>
        public IdentityUserDbContext(DbContextOptions<IdentityUserDbContext> options) : base(options)
        {
        }
    }
}