using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Tardigrade.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the enum type.
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Get the Description value of the Display attribute associated with the enumerated type (if exists).
        /// </summary>
        /// <param name="enumeration">Enumerated type.</param>
        /// <returns>Description value if exists; enumeration value as a string otherwise.</returns>
        public static string ToDescription(this Enum enumeration)
        {
            if (enumeration == null) throw new ArgumentNullException(nameof(enumeration));

            var descriptionAttribute = (DisplayAttribute)enumeration
                .GetType()
                .GetField(enumeration.ToString())
                ?.GetCustomAttributes(false)
                .FirstOrDefault(a => a is DisplayAttribute);
            string description = (descriptionAttribute != null ? descriptionAttribute.Description : enumeration.ToString());

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
        /// <exception cref="InvalidOperationException">The TEnum generic type is not an enumerated type.</exception>
        public static T ToEnum<T>(this int value, T defaultValue = default) where T : struct, IComparable, IConvertible, IFormattable
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
        /// Convert the string value into the equivalent enumerated type value. If the string value is not within a
        /// valid range, throw ArgumentException.
        /// </summary>
        /// <typeparam name="T">Enumerated type.</typeparam>
        /// <param name="value">String value to convert.</param>
        /// <returns>Enumerated type value.</returns>
        /// <exception cref="ArgumentException">Value is not valid for the enumerated type.</exception>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        /// <exception cref="InvalidOperationException">The T generic type is not an enumerated type.</exception>
        public static T ToEnum<T>(this string value) where T : struct, IComparable, IConvertible, IFormattable
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException("This operation is only applicable for enumerated types.");
            }

            if (!Enum.TryParse(value, true, out T enumValue) || !Enum.IsDefined(typeof(T), enumValue))
            {
                throw new ArgumentException($"Value '{value}' is not valid for the {typeof(T).Name} enumeration.");
            }

            return enumValue;
        }

        /// <summary>
        /// Convert the string value into the equivalent enumerated type value. If the string value is not within a
        /// valid range, the default value is used to set the enumerated type.
        /// </summary>
        /// <typeparam name="T">Enumerated type.</typeparam>
        /// <param name="value">String value to convert.</param>
        /// <param name="defaultValue">Default enumerated type value to use if string value is not within a valid range.</param>
        /// <returns>Enumerated type value.</returns>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        /// <exception cref="InvalidOperationException">The T generic type is not an enumerated type.</exception>
        public static T ToEnum<T>(this string value, T defaultValue)
            where T : struct, IComparable, IConvertible, IFormattable
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException("This operation is only applicable for enumerated types.");
            }

            if (!Enum.TryParse(value, true, out T enumValue) || !Enum.IsDefined(typeof(T), enumValue))
            {
                return defaultValue;
            }

            return enumValue;
        }

        /// <summary>
        /// Get the Name value of the Display attribute associated with the enumerated type (if exists).
        /// </summary>
        /// <param name="enumeration">Enumerated type.</param>
        /// <returns>Name value if exists; enumeration value as a string otherwise.</returns>
        public static string ToName(this Enum enumeration)
        {
            if (enumeration == null) throw new ArgumentNullException(nameof(enumeration));

            var displayAttribute = (DisplayAttribute)enumeration
                .GetType()
                .GetField(enumeration.ToString())
                ?.GetCustomAttributes(false)
                .FirstOrDefault(a => a is DisplayAttribute);
            string name = (displayAttribute != null ? displayAttribute.Name : enumeration.ToString());

            return name;
        }
    }
}