using Microsoft.Extensions.Configuration;
using System;
using System.Text.RegularExpressions;
using Tardigrade.Framework.Exceptions;

namespace Tardigrade.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the IConfiguration interface.
    /// All extension methods in this class support the use of referenced application settings. Refer to the the
    /// <see cref="GetAsString"/> method for more information.
    /// </summary>
    public static class ConfigurationExtension
    {
        private const string Pattern = @"\$\{([\w\.\-_]+)\}";

        private static readonly Regex Regex = new(Pattern, RegexOptions.IgnoreCase);

        /// <summary>
        /// Get the Boolean value for an application setting. If it does not exist, then the default value is returned.
        /// This method leverages the <see cref="GetAsString"/> method.
        /// </summary>
        /// <param name="configuration">IConfiguration associated with this extension.</param>
        /// <param name="name">Name of the application setting.</param>
        /// <param name="defaultValue">Default value returned in case the application setting does not exist.</param>
        /// <returns>Value associated with the application setting if it exists; the default value otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null or empty.</exception>
        /// <exception cref="FormatException">Value associated with the application setting is not a Boolean.</exception>
        /// <exception cref="NotFoundException">A referenced application setting does not exist (if specified).</exception>
        public static bool? GetAsBoolean(this IConfiguration configuration, string name, bool? defaultValue = null)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            bool? booleanValue = defaultValue;
            string stringValue = configuration.GetAsString(name);

            if (stringValue != null)
            {
                booleanValue = bool.Parse(stringValue);
            }

            return booleanValue;
        }

        /// <summary>
        /// Get the Enum value for an application setting (not case sensitive). If it does not exist, then the default
        /// value is returned.
        /// This method leverages the <see cref="GetAsString"/> method.
        /// </summary>
        /// <typeparam name="TEnum">Type of the enumeration.</typeparam>
        /// <param name="configuration">IConfiguration associated with this extension.</param>
        /// <param name="name">Name of the application setting.</param>
        /// <param name="defaultValue">Default value returned in case the application setting does not exist.</param>
        /// <returns>Value associated with the application setting if it exists; the default value otherwise.</returns>
        /// <exception cref="FormatException">Value associated with the application setting is not an enumeration type.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null or empty.</exception>
        /// <exception cref="NotFoundException">A referenced application setting does not exist (if specified).</exception>
        /// <exception cref="InvalidOperationException">The TEnum generic type is not an enumerated type.</exception>
        public static TEnum? GetAsEnum<TEnum>(
            this IConfiguration configuration,
            string name,
            TEnum? defaultValue = null)
            where TEnum : struct, IConvertible
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            if (!typeof(TEnum).IsEnum)
            {
                throw new InvalidOperationException("This operation is only applicable for enumerated types.");
            }

            string stringValue = configuration.GetAsString(name);

            if (stringValue == null)
            {
                return defaultValue;
            }

            if (!Enum.TryParse(stringValue, true, out TEnum enumValue) || !Enum.IsDefined(typeof(TEnum), enumValue))
            {
                throw new FormatException(
                    $"Value '{stringValue}' is not valid for setting {name} as it is not an underlying value of the {typeof(TEnum).Name} enumeration.");
            }

            return enumValue;
        }

        /// <summary>
        /// Get the GUID value for an application setting. If it does not exist, then the default value is returned.
        /// This method leverages the <see cref="GetAsString"/> method.
        /// </summary>
        /// <param name="configuration">IConfiguration associated with this extension.</param>
        /// <param name="name">Name of the application setting.</param>
        /// <param name="defaultValue">Default value returned in case the application setting does not exist.</param>
        /// <returns>Value associated with the application setting if it exists; the default value otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null or empty.</exception>
        /// <exception cref="FormatException">Value associated with the application setting is not a GUID.</exception>
        /// <exception cref="NotFoundException">A referenced application setting does not exist (if specified).</exception>
        public static Guid? GetAsGuid(this IConfiguration configuration, string name, Guid? defaultValue = null)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            Guid? intValue = defaultValue;
            string stringValue = configuration.GetAsString(name);

            if (stringValue != null)
            {
                intValue = Guid.Parse(stringValue);
            }

            return intValue;
        }

        /// <summary>
        /// Get the Integer value for an application setting. If it does not exist, then the default value is returned.
        /// This method leverages the <see cref="GetAsString"/> method.
        /// </summary>
        /// <param name="configuration">IConfiguration associated with this extension.</param>
        /// <param name="name">Name of the application setting.</param>
        /// <param name="defaultValue">Default value returned in case the application setting does not exist.</param>
        /// <returns>Value associated with the application setting if it exists; the default value otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null or empty.</exception>
        /// <exception cref="FormatException">Value associated with the application setting is not an Integer.</exception>
        /// <exception cref="NotFoundException">A referenced application setting does not exist (if specified).</exception>
        /// <exception cref="OverflowException">Value associated with the application setting is either too large or too small for an Integer.</exception>
        public static int? GetAsInt(this IConfiguration configuration, string name, int? defaultValue = null)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            int? intValue = defaultValue;
            string stringValue = configuration.GetAsString(name);

            if (stringValue != null)
            {
                intValue = int.Parse(stringValue);
            }

            return intValue;
        }

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
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null or empty.</exception>
        /// <exception cref="NotFoundException">A referenced application setting does not exist (if specified).</exception>
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
                    string referencedValue = configuration.GetAsString(referencedName)
                        ?? throw new NotFoundException($"Referenced application setting {referencedName.Trim()} does not exist.");
                    value = Regex.Replace(value, referencedValue, 1);
                }
            }

            return value;
        }

        /// <summary>
        /// Get the URI value for an application setting. If it does not exist, then the default value is returned.
        /// This method leverages the <see cref="GetAsString"/> method.
        /// </summary>
        /// <param name="configuration">IConfiguration associated with this extension.</param>
        /// <param name="name">Name of the application setting.</param>
        /// <param name="defaultValue">Default value returned in case the application setting does not exist.</param>
        /// <returns>Value associated with the application setting if it exists; the default value otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null or empty.</exception>
        /// <exception cref="NotFoundException">A referenced application setting does not exist (if specified).</exception>
        /// <exception cref="UriFormatException">Value associated with the application setting is not a URI.</exception>
        public static Uri GetAsUri(this IConfiguration configuration, string name, Uri defaultValue = null)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            Uri uriValue = defaultValue;
            string stringValue = configuration.GetAsString(name);

            if (stringValue != null)
            {
                uriValue = new Uri(stringValue);
            }

            return uriValue;
        }
    }
}