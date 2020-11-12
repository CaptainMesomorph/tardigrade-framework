using Microsoft.Extensions.Configuration;
using System;
using System.Text.RegularExpressions;
using Tardigrade.Framework.Exceptions;

namespace Tardigrade.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the IConfiguration interface.
    /// </summary>
    public static class ConfigurationExtension
    {
        private const string Pattern = @"\$\{([\w\.\-_]+)\}";

        private static readonly Regex Regex = new Regex(Pattern, RegexOptions.IgnoreCase);

        /// <summary>
        /// If the application setting exists, return the value associated with it. If it does not exist, then the
        /// default value is returned.
        /// An application setting value may itself reference another (existing) application setting using the
        /// syntax ${}. For example:
        /// <![CDATA[
        ///     <add key="Referenced.Setting" value="World"/>
        ///     <add key="Print.Statement" value="Hello ${Referenced.Setting}"/>
        /// ]]>
        /// In this case, getting the "Print.Statement" application setting will return the value "Hello World".
        /// </summary>
        /// <param name="configuration">IConfiguration associated with this extension.</param>
        /// <param name="name">Name of the application setting.</param>
        /// <param name="defaultValue">Default value returned in case the application setting does not exist.</param>
        /// <returns>Value associated with the application setting if it exists; the default value otherwise.</returns>
        /// <exception cref="ArgumentNullException">configuration is null; name is null or empty.</exception>
        /// <exception cref="NotFoundException">Referenced application setting does not exist.</exception>
        public static string GetAsString(this IConfiguration configuration, string name, string defaultValue = null)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            string value = configuration[name.Trim()];

            if (value == null)
            {
                value = defaultValue;
            }
            else
            {
                foreach (Match match in Regex.Matches(value))
                {
                    string referencedName = match.Groups[1].Value;
                    string referencedValue = configuration.GetAsString(referencedName);

                    if (referencedValue == null)
                    {
                        throw new NotFoundException($"Referenced application setting {referencedName.Trim()} does not exist.");
                    }

                    value = Regex.Replace(value, referencedValue, 1);
                }
            }

            return value;
        }
    }
}