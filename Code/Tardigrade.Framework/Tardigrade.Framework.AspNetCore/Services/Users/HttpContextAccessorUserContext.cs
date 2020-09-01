using Microsoft.AspNetCore.Http;
using System;
using Tardigrade.Framework.Services.Users;

namespace Tardigrade.Framework.AspNetCore.Services.Users
{
    /// <summary>
    /// User context class based upon the IHttpContextAccessor interface.
    /// </summary>
    public class HttpContextAccessorUserContext : IUserContext
    {
        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="httpContextAccessor">Mechanism for accessing HttpContext.</param>
        public HttpContextAccessorUserContext(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null) throw new ArgumentNullException(nameof(httpContextAccessor));

            // Unable to support user IDs until all applications are ported to .NET Core.
            //CurrentUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            //              throw new ArgumentException(
            //                  "Unable to determine the unique identifier of the current user.",
            //                  nameof(httpContextAccessor));
            CurrentUser = httpContextAccessor.HttpContext.User.Identity.Name ??
                          throw new ArgumentException("Unable to determine the name of the current user.",
                              nameof(httpContextAccessor));
        }

        /// <inheritdoc />
        public string CurrentUser { get; }
    }
}