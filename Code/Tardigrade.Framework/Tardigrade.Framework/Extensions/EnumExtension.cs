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
            DisplayAttribute descriptionAttribute = (DisplayAttribute)enumeration.GetType()
                .GetField(enumeration.ToString())
                .GetCustomAttributes(false)
                .Where(a => a is DisplayAttribute)
                .FirstOrDefault();
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
        public static T ToEnum<T>(this int value, T defaultValue = default(T)) where T : struct, IComparable, IConvertible, IFormattable
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
        /// valid range, the default value is used to set the enumerated type.
        /// </summary>
        /// <typeparam name="T">Enumerated type.</typeparam>
        /// <param name="value">String value to convert.</param>
        /// <param name="defaultValue">Default enumerated type value to use if string value is not within a valid range.</param>
        /// <returns>Enumerated type value.</returns>
        public static T ToEnum<T>(this string value, T defaultValue = default(T)) where T : struct, IComparable, IConvertible, IFormattable
        {
            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException("This operation is only applicable for enumerated types.");
            }

            if (!Enum.TryParse(value, true, out T enumeration))
            {
                enumeration = defaultValue;
            }

            return enumeration;
        }

        /// <summary>
        /// Get the Name value of the Display attribute associated with the enumerated type (if exists).
        /// </summary>
        /// <param name="enumeration">Enumerated type.</param>
        /// <returns>Name value if exists; enumeration value as a string otherwise.</returns>
        public static string ToName(this Enum enumeration)
        {
            DisplayAttribute displayAttribute = (DisplayAttribute)enumeration.GetType()
                .GetField(enumeration.ToString())
                .GetCustomAttributes(false)
                .Where(a => a is DisplayAttribute)
                .FirstOrDefault();
            string name = (displayAttribute != null ? displayAttribute.Name : enumeration.ToString());

            return name;
        }
    }
}