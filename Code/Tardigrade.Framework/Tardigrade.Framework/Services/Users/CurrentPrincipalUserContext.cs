using System.Threading;

namespace Tardigrade.Framework.Services.Users
{
    /// <summary>
    /// <see cref="IUserContext"/>
    /// </summary>
    public class CurrentPrincipalUserContext : IUserContext
    {
        /// <summary>
        /// <see cref="IUserContext.CurrentUser"/>
        /// </summary>
        public string CurrentUser => Thread.CurrentPrincipal?.Identity?.Name;
    }
}