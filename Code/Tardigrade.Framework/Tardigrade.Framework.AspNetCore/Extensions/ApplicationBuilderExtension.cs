using Microsoft.AspNetCore.Builder;
using Tardigrade.Framework.AspNetCore.Middlewares;

namespace Tardigrade.Framework.AspNetCore.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the IApplicationBuilder interface.
    /// </summary>
    public static class ApplicationBuilderExtension
    {
        /// <summary>
        /// Add middleware for managing the current tenant.
        /// </summary>
        /// <param name="builder">Application builder.</param>
        /// <returns>Application builder.</returns>
        public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantMiddleware>();
        }
    }
}