using System;

namespace Tardigrade.Framework.Tenants
{
    /// <summary>
    /// Configuration settings associated with tenanting.
    /// </summary>
    public interface ITenantConfiguration
    {
        /// <summary>
        /// Retrieve a tenant based upon a key/value pair mapping. If a mapping for the specified key does not exist,
        /// the default mapping value will be returned.
        /// </summary>
        /// <param name="key">Key used in the tenant mapping.</param>
        /// <param name="defaultValue">Default mapping value used if the specified key does not exist.</param>
        /// <returns>Tenant associated with the mapping key.</returns>
        /// <exception cref="ArgumentNullException">key is either null or empty.</exception>
        string GetTenantMapping(string key, string defaultValue = null);
    }
}