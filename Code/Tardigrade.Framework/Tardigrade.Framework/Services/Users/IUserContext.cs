namespace Tardigrade.Framework.Services.Users
{
    /// <summary>
    /// Contextual information about the currently signed in user.
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// Currently signed in user.
        /// </summary>
        string CurrentUser { get; }
    }
}