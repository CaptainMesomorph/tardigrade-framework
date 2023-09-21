using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Tardigrade.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the enum type.
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Get a specific attribute associated with the enumeration.
        /// </summary>
        /// <typeparam name="TAttribute">Attribute to get.</typeparam>
        /// <param name="enumeration">Enumeration to check.</param>
        /// <returns>Attribute associated with the enumeration if exists; null otherwise.</returns>
        private static TAttribute GetAttribute<TAttribute>(Enum enumeration) where TAttribute : Attribute
        {
            TAttribute attribute = enumeration
                .GetType()
                .GetField(enumeration.ToString())
                ?.GetCustomAttributes(typeof(TAttribute), false)
                .FirstOrDefault() as TAttribute;

            return attribute;
        }

        /// <summary>
        /// Convert the string value into the equivalent enumerated type value. If conversion fails and there is an
        /// associated Display attribute for the enumerated type value, then it's Name property will also be evaluated.
        /// <a href="https://codereview.stackexchange.com/questions/5352/getting-the-value-of-a-custom-attribute-from-an-enum">Getting the value of a custom attribute from an enum</a>
        /// </summary>
        /// <typeparam name="T">Enumerated type.</typeparam>
        /// <param name="value">String value to convert.</param>
        /// <returns>Enumerated type if value is valid; null otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The T generic type is not an enumerated type.</exception>
        private static T? GetEnum<T>(string value) where T : struct, IComparable, IConvertible, IFormattable
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException("This operation is only applicable for enumerated types.");
            }

            // Check whether the string value represents a valid enumerated type value.
            if (Enum.TryParse(value, true, out T enumValue) && Enum.IsDefined(typeof(T), enumValue))
            {
                return enumValue;
            }

            T? result = null;

            // Check whether the string value represents the Name property of the Display attribute of the enumerated
            // type value.
            foreach (T typeValue in Enum.GetValues(typeof(T)))
            {
                string displayName = typeValue
                    .GetType()
                    .GetField(typeValue.ToString() ?? string.Empty)?
                    .GetCustomAttribute<DisplayAttribute>(false)?
                    .Name;

                if (value.Equals(displayName))
                {
                    result = typeValue;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Get the Description value of the attribute associated with the enumerated type. This method is only
        /// applicable for the <see cref="DisplayAttribute"/> and <see cref="DescriptionAttribute"/> types. For any
        /// other attribute type, the enumeration value as a string is returned.
        /// </summary>
        /// <param name="enumeration">Enumerated type.</param>
        /// <returns>Description value if exists; enumeration value as a string otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumeration"/> is null.</exception>
        public static string ToDescription(this Enum enumeration)
        {
            if (enumeration == null)
            {
                throw new ArgumentNullException(nameof(enumeration));
            }

            DescriptionAttribute descriptionAttribute = GetAttribute<DescriptionAttribute>(enumeration);
            string description;

            if (descriptionAttribute == null)
            {
                DisplayAttribute displayAttribute = GetAttribute<DisplayAttribute>(enumeration);
                description = displayAttribute == null ? enumeration.ToString() : displayAttribute.Description;
            }
            else
            {
                description = descriptionAttribute.Description;
            }

            return description;
        }

        /// <summary>
        /// Convert the integer value into the equivalent enumerated type value. If the integer value is not within a
        /// valid range, the default value is used to set the enumerated type.
        /// </summary>
        /// <typeparam name="T">Enumerated type.</typeparam>
        /// <param name="value">Integer value to convert.</param>
        /// <param name="defaultValue">Default enumerated type value to use if integer value is not within a valid range.</param>
        /// <returns>Enumerated type value.</returns>
        /// <exception cref="InvalidOperationException">The T generic type is not an enumerated type.</exception>
        public static T ToEnum<T>(this int value, T defaultValue = default)
            where T : struct, IComparable, IConvertible, IFormattable
        {
            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException("This operation is only applicable for enumerated types.");
            }

            T enumeration = defaultValue;

            if (Enum.IsDefined(typeof(T), value))
            {
                enumeration = (T)Enum.Parse(typeof(T), value.ToString());
            }

            return enumeration;
        }

        /// <summary>
        /// Convert the string value into the equivalent enumerated type value. If conversion fails and there is an
        /// associated Display attribute for the enumerated type value, then it's Name property will also be evaluated.
        /// If the string value is not within a valid range, throw ArgumentException.
        /// </summary>
        /// <typeparam name="T">Enumerated type.</typeparam>
        /// <param name="value">String value to convert.</param>
        /// <returns>Enumerated type value.</returns>
        /// <exception cref="ArgumentException">Value is not valid for the enumerated type.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The T generic type is not an enumerated type.</exception>
        public static T ToEnum<T>(this string value) where T : struct, IComparable, IConvertible, IFormattable
        {
            return GetEnum<T>(value) ??
                   throw new ArgumentException($"Value '{value}' is not valid for the {typeof(T).Name} enumeration.");
        }

        /// <summary>
        /// Convert the string value into the equivalent enumerated type value. If conversion fails and there is an
        /// associated Display attribute for the enumerated type value, then it's Name property will also be evaluated.
        /// If the string value is not within a valid range, the default value is returned.
        /// </summary>
        /// <typeparam name="T">Enumerated type.</typeparam>
        /// <param name="value">String value to convert.</param>
        /// <param name="defaultValue">Default enumerated type value to use if string value is not within a valid range.</param>
        /// <returns>Enumerated type value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The T generic type is not an enumerated type.</exception>
        public static T ToEnum<T>(this string value, T defaultValue)
            where T : struct, IComparable, IConvertible, IFormattable
        {
            return GetEnum<T>(value) ?? defaultValue;
        }

        /// <summary>
        /// Get the Name value of the Display attribute associated with the enumerated type (if exists).
        /// </summary>
        /// <param name="enumeration">Enumerated type.</param>
        /// <returns>Name value if exists; enumeration value as a string otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumeration"/> is null.</exception>
        public static string ToName(this Enum enumeration)
        {
            if (enumeration == null)
            {
                throw new ArgumentNullException(nameof(enumeration));
            }

            DisplayAttribute displayAttribute = GetAttribute<DisplayAttribute>(enumeration);
            string name = displayAttribute == null ? enumeration.ToString() : displayAttribute.Name;

            return name;
        }
    }
}