namespace Tardigrade.Framework.AzureStorage.Models
{
    /// <summary>
    /// Interface definition for the primary key of a Storage Table.
    /// </summary>
    public interface ITableKey
    {
        /// <summary>
        /// Storage Table partition key.
        /// </summary>
        string Partition { get; }

        /// <summary>
        /// Storage Table row key.
        /// </summary>
        string Row { get; }
    }
}