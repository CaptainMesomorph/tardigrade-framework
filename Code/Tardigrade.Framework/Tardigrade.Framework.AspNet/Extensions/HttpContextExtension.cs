using System;
using System.Web;

namespace Tardigrade.Framework.AspNet.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the HttpContextBase type.
    /// </summary>
    public static class HttpContextExtension
    {
#if NET472

        /// <summary>
        /// Create a cookie for the browser by adding it to the HTTP Response.
        /// </summary>
        /// <param name="httpContext">HTTP context.</param>
        /// <param name="name">Name of the cookie.</param>
        /// <param name="value">Value associated with the cookie.</param>
        /// <param name="domain">Domain associated with the cookie.</param>
        /// <param name="expires">Expiration date and time for the cookie.</param>
        /// <param name="sameSite">Same site enforcement mode associated with the cookie.</param>
        /// <exception cref="ArgumentNullException">Cookie name is either null or empty.</exception>
        public static void CookieCreate(this HttpContextBase httpContext,
            string name,
            string value,
            string domain = null,
            DateTime? expires = null,
            SameSiteMode sameSite = SameSiteMode.None)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            HttpCookie cookie = new HttpCookie(name)
            {
                SameSite = sameSite,
                Value = value
            };

            if (!string.IsNullOrWhiteSpace(domain))
            {
                cookie.Domain = domain;
            }

            if (expires.HasValue)
            {
                cookie.Expires = expires.Value;
            }

            httpContext.Response.Cookies.Add(cookie);
        }

#else

        /// <summary>
        /// Create a cookie for the browser by adding it to the HTTP Response.
        /// </summary>
        /// <param name="httpContext">HTTP context.</param>
        /// <param name="name">Name of the cookie.</param>
        /// <param name="value">Value associated with the cookie.</param>
        /// <param name="domain">Domain associated with the cookie.</param>
        /// <param name="expires">Expiration date and time for the cookie.</param>
        /// <exception cref="ArgumentNullException">Cookie name is either null or empty.</exception>
        public static void CookieCreate(this HttpContextBase httpContext,
            string name,
            string value,
            string domain = null,
            DateTime? expires = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            HttpCookie cookie = new HttpCookie(name)
            {
                Value = value
            };

            if (!string.IsNullOrWhiteSpace(domain))
            {
                cookie.Domain = domain;
            }

            if (expires.HasValue)
            {
                cookie.Expires = expires.Value;
            }

            httpContext.Response.Cookies.Add(cookie);
        }

#endif

        /// <summary>
        /// Delete the cookie from the browser. Both the cookie name and domain must match the cookie to delete.
        /// </summary>
        /// <param name="httpContext">HTTP context.</param>
        /// <param name="name">Name of the cookie.</param>
        /// <param name="domain">Domain associated with the cookie.</param>
        /// <exception cref="ArgumentNullException">Cookie name is either null or empty.</exception>
        public static void CookieDelete(this HttpContextBase httpContext, string name, string domain = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            HttpCookie cookie = httpContext.Request.Cookies[name];

            if (cookie != null)
            {
                cookie.Expires = DateTime.UtcNow.AddDays(-1);

                if (!string.IsNullOrWhiteSpace(domain))
                {
                    cookie.Domain = domain;
                }

                httpContext.Response.Cookies.Add(cookie);
                httpContext.Request.Cookies.Remove(name);
            }
        }

        /// <summary>
        /// Check whether a browser cookie exists.
        /// </summary>
        /// <param name="httpContext">HTTP context.</param>
        /// <param name="name">Name of the cookie.</param>
        /// <returns>True if the browser cookie exists; false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Cookie name is either null or empty.</exception>
        public static bool CookieExists(this HttpContextBase httpContext, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            return httpContext.Request.Cookies[name] != null;
        }

        /// <summary>
        /// Retrieve the browser cookie.
        /// </summary>
        /// <param name="httpContext">HTTP context.</param>
        /// <param name="name">Name of the cookie.</param>
        /// <returns>Cookie if it exists; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">Cookie name is either null or empty.</exception>
        public static HttpCookie CookieRetrieve(this HttpContextBase httpContext, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            return httpContext.Request.Cookies[name];
        }

        /// <summary>
        /// Retrieve the value associated with the browser cookie.
        /// </summary>
        /// <param name="httpContext">HTTP context.</param>
        /// <param name="name">Name of the cookie.</param>
        /// <returns>Value associated with the cookie if it exists; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">Cookie name is either null or empty.</exception>
        public static string CookieRetrieveValue(this HttpContextBase httpContext, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            string value = null;
            HttpCookie cookie = httpContext.Request.Cookies[name];

            if (!string.IsNullOrEmpty(cookie?.Value))
            {
                value = Uri.UnescapeDataString(cookie.Value);
            }

            return value;
        }
    }
}