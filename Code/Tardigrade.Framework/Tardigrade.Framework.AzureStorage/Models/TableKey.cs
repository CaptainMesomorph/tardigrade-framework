namespace Tardigrade.Framework.AzureStorage.Models
{
    /// <summary>
    /// Primary key of a Storage Table.
    /// </summary>
    public class TableKey
    {
        /// <summary>
        /// <see cref="ITableKey.Partition"/>
        /// </summary>
        public string Partition { get; protected set; }

        /// <summary>
        /// <see cref="ITableKey.Row"/>
        /// </summary>
        public string Row { get; protected set; }
    }
}