using Microsoft.EntityFrameworkCore;
using Tardigrade.Framework.Models.Settings;

namespace Tardigrade.Framework.EntityFrameworkCore.Data
{
    /// <summary>
    /// Entity Framework Core database context for use with application settings stored in a database.
    /// </summary>
    public class AppSettingsDbContext : DbContext
    {
        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="options">Options associated with the database context.</param>
        public AppSettingsDbContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// Application settings.
        /// </summary>
        public DbSet<AppSetting> AppSettings { get; set; }
    }
}