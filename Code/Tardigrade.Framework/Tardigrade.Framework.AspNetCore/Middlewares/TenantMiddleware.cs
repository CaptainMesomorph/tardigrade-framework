using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Tardigrade.Framework.Tenants;

namespace Tardigrade.Framework.AspNetCore.Middlewares
{
    /// <inheritdoc />
    public class TenantMiddleware : IMiddleware
    {
        private readonly ITenantResolver<string> _tenantResolver;
        private readonly ITenantService<string> _tenantService;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="tenantResolver">Tenant resolver.</param>
        /// <param name="tenantService">Tenant service.</param>
        public TenantMiddleware(ITenantResolver<string> tenantResolver, ITenantService<string> tenantService)
        {
            _tenantResolver = tenantResolver;
            _tenantService = tenantService;
        }

        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string tenant = _tenantResolver.Tenant;

            if (string.IsNullOrWhiteSpace(tenant)) throw new InvalidOperationException("Tenant cannot be resolved.");

            await _tenantService.SaveAsync(tenant.Trim());
            await next(context);
        }
    }
}