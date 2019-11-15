using System;
using System.Configuration;
using System.Text.RegularExpressions;
using Tardigrade.Framework.Exceptions;

namespace Tardigrade.Framework.Helpers
{
    /// <summary>
    /// Helper class to extend the functionality of the ConfigurationManager class.
    /// </summary>
    public static class ConfigurationManagerHelper
    {
        private const string pattern = @"\$\{([\w\.\-_]+)\}";

        private static readonly Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

        /// <summary>
        /// If the application setting exists, return the value associated with it. If it does not exist, then the
        /// default value is returned.
        /// An application setting value may itself reference another (existing) application setting using the
        /// syntax ${}. For example:
        /// <![CDATA[
        ///     <add key="Referenced.Setting" value="World"/>
        ///     <add key="Print.Statement" value="Hello ${Referenced.Setting}"/>
        /// ]]>
        /// In this case, getting the "Print.Statement" applicaton setting will return the value "Hello World".
        /// </summary>
        /// <param name="name">Name of the application setting.</param>
        /// <param name="defaultValue">Default value returned in case the application setting does not exist.</param>
        /// <returns>Value associated with the application setting if it exists; the default value otherwise.</returns>
        /// <exception cref="ArgumentNullException">name is null or empty.</exception>
        /// <exception cref="NotFoundException">Referenced application setting does not exist.</exception>
        public static string GetStringSetting(string name, string defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            string value = ConfigurationManager.AppSettings[name.Trim()];

            if (value == null)
            {
                value = defaultValue;
            }
            else
            {
                foreach (Match match in regex.Matches(value))
                {
                    string referencedName = match.Groups[1].Value;
                    string referencedValue = GetStringSetting(referencedName);

                    if (referencedValue == null)
                    {
                        throw new NotFoundException($"Referenced application setting {referencedName.Trim()} does not exist.");
                    }

                    value = regex.Replace(value, referencedValue, 1);
                }
            }

            return value;
        }
    }
}