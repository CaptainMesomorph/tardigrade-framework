using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using Tardigrade.Framework.Models.Responses;

namespace Tardigrade.Framework.AspNetCore.Extensions
{
    /// <summary>
    /// Static class containing extension methods for the ControllerBase class.
    /// </summary>
    public static class ControllerBaseExtension
    {
        /// <summary>
        /// Create a BadRequest object result based upon the message provided.
        /// </summary>
        /// <param name="source">ControllerBase class extension is applicable for.</param>
        /// <param name="message">Message associated with the BadRequest.</param>
        /// <returns>BadRequest object result based upon the message provided</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static BadRequestObjectResult BadRequest(this ControllerBase source, string message)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            source.ModelState.AddModelError(string.Empty, message);
            return source.BadRequest(source.ModelState);
        }

        /// <summary>
        /// Create an object result based upon a status code and provided message.
        /// </summary>
        /// <param name="source">ControllerBase class extension is applicable for.</param>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="message">Message associated with the object result.</param>
        /// <returns>Object result based upon a status code and provided message</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public static ObjectResult StatusCode(this ControllerBase source, int statusCode, string message)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            ResponseObject responseObject = new ResponseObject
            {
                Message = message,
                RequestUrl = $"{source.Request.Method} {source.Request.GetDisplayUrl()}",
                StatusCode = statusCode,
                StatusDescription = ReasonPhrases.GetReasonPhrase(statusCode)
            };

            return source.StatusCode(statusCode, responseObject);
        }
    }
}