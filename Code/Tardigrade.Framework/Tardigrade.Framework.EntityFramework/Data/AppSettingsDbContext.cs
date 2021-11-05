using System.Data.Entity;
using Tardigrade.Framework.Models.Settings;

namespace Tardigrade.Framework.EntityFramework.Data
{
    /// <summary>
    /// Entity Framework database context for use with application settings stored in a database.
    /// </summary>
    public class AppSettingsDbContext : DbContext
    {
        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="nameOrConnectionString">Name of the database connection or database connection string.</param>
        public AppSettingsDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        /// <summary>
        /// Application settings.
        /// </summary>
        public DbSet<AppSetting> AppSettings { get; set; }
    }
}