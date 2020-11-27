using Tardigrade.Framework.Models.Domain;

namespace Tardigrade.Framework.Models.Settings
{
    /// <summary>
    /// Class that represents an application setting as a key value pair.
    /// </summary>
    public class AppSetting : IHasUniqueIdentifier<string>
    {
        /// <summary>
        /// Setting name or key.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Setting value,
        /// </summary>
        public string Value { get; set; }
    }
}