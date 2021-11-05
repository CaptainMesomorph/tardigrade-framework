namespace Tardigrade.Framework.Tenants
{
    /// <summary>
    /// Resolver interface for determining the current tenant.
    /// </summary>
    public interface ITenantResolver<out T>
    {
        /// <summary>
        /// Current tenant.
        /// </summary>
        T Tenant { get; }
    }
}