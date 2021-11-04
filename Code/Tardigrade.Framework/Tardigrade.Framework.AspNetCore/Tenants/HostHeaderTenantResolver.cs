using Microsoft.AspNetCore.Http;
using System;
using Tardigrade.Framework.Tenants;

namespace Tardigrade.Framework.AspNetCore.Tenants
{
    /// <inheritdoc />
    public class HostHeaderTenantResolver : ITenantResolver<string>
    {
        private readonly ITenantConfiguration _config;
        private readonly HttpContext _httpContext;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="configuration">Application configuration properties.</param>
        /// <param name="httpContextAccessor">Class for accessing the HTTP context.</param>
        /// <exception cref="ArgumentNullException">httpContextAccessor or it's associated context is null.</exception>
        public HostHeaderTenantResolver(ITenantConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContext =
                httpContextAccessor?.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <inheritdoc />
        public string Tenant => _config.GetTenantMapping(_httpContext.Request.Host.Host);
    }
}