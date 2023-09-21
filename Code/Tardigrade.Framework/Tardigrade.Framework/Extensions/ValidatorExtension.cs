using System;
using System.ComponentModel.DataAnnotations;

namespace Tardigrade.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the validation of model objects.
    /// </summary>
    public static class ValidatorExtension
    {
        /// <summary>
        /// Validate the model object.
        /// </summary>
        /// <typeparam name="T">Type of the model object.</typeparam>
        /// <param name="obj">Model object to validate.</param>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is null.</exception>"
        /// <exception cref="ValidationException"><paramref name="obj" /> is invalid.</exception>
        public static void Validate<T>(this T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var context = new ValidationContext(obj);
            Validator.ValidateObject(obj, context);
        }
    }
}