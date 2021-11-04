namespace Tardigrade.Framework.Tenants
{
    /// <summary>
    /// Interface for defining whether a domain model has an associated tenant.
    /// </summary>
    public interface IHasTenant<T>
    {
        /// <summary>
        /// Tenant.
        /// </summary>
        T Tenant { get; set; }
    }
}