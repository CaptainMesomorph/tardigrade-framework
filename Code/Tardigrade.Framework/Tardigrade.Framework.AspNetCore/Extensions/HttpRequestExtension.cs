using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;

namespace Tardigrade.Framework.AspNetCore.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the HttpRequest class.
    /// </summary>
    public static class HttpRequestExtension
    {
        /// <summary>
        /// Get the scheme and authority from the current HTTP request context (e.g. https://www.blah.com.au).
        /// </summary>
        /// <param name="request">Current HTTP request context.</param>
        /// <returns>Scheme and authority associated with the current HTTP request context.</returns>
        public static string SchemeAndAuthority(this HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var displayUrl = new Uri(request.GetDisplayUrl());

            return displayUrl.GetLeftPart(UriPartial.Authority);
        }
    }
}