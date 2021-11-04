using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tardigrade.Framework.Tenants;

namespace Tardigrade.Framework.AspNetCore.Tenants
{
    /// <inheritdoc />
    public class SessionTenantService : ITenantService<string>
    {
        private readonly HttpContext _httpContext;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="httpContextAccessor">Class for accessing the HTTP context.</param>
        /// <exception cref="ArgumentNullException">httpContextAccessor is null.</exception>
        public SessionTenantService(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null) throw new ArgumentNullException(nameof(httpContextAccessor));

            _httpContext = httpContextAccessor.HttpContext;
        }

        /// <inheritdoc />
        public void Delete()
        {
            _httpContext.Session.Remove(TenantCookie.TenantIdKey);
        }

        /// <inheritdoc />
        public async Task DeleteAsync()
        {
            await Task.Run(Delete);
        }

        /// <inheritdoc />
        public bool Exists()
        {
            return _httpContext.Session.Keys.Contains(TenantCookie.TenantIdKey);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync()
        {
            return await Task.Run(Exists);
        }

        /// <inheritdoc />
        public string Retrieve()
        {
            return _httpContext.Session.GetString(TenantCookie.TenantIdKey);
        }

        /// <inheritdoc />
        public async Task<string> RetrieveAsync()
        {
            return await Task.Run(Retrieve);
        }

        /// <inheritdoc />
        public void Save(string tenant)
        {
            if (string.IsNullOrWhiteSpace(tenant)) throw new ArgumentNullException(nameof(tenant));

            _httpContext.Session.SetString(TenantCookie.TenantIdKey, tenant);
        }

        /// <inheritdoc />
        public async Task SaveAsync(string tenant)
        {
            await Task.Run(() => Save(tenant));
        }
    }
}