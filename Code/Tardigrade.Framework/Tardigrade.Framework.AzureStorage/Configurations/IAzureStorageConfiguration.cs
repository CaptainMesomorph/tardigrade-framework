namespace Tardigrade.Framework.AzureStorage.Configurations
{
    /// <summary>
    /// Configuration application properties associated with Azure Storage.
    /// </summary>
    public interface IAzureStorageConfiguration
    {
        /// <summary>
        /// Azure Storage connection string.
        /// </summary>
        string AzureStorageConnectionString { get; set; }
    }
}