using System.Threading.Tasks;

namespace Tardigrade.Framework.Services.Identity
{
    /// <summary>
    /// Interface that defines operations associated with application users.
    /// </summary>
    /// <typeparam name="IApplicationUser">Type associated with the application user definition.</typeparam>
    public interface IApplicationUserManager<IApplicationUser>
    {
        /// <summary>
        /// Check whether the given password is valid for the specified application user.
        /// </summary>
        /// <param name="user">Application user whose password should be validated.</param>
        /// <param name="password">Password to validate.</param>
        /// <returns>True if the specified password matches the one stored; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">user is null, or password is null or empty.</exception>
        Task<bool> CheckPasswordAsync(IApplicationUser user, string password);

        /// <summary>
        /// Create the specified application user with the password provided.
        /// TODO: Create a wrapper for IdentityResult.
        /// </summary>
        /// <param name="user">Application user to create.</param>
        /// <param name="password">Password for the application user.</param>
        /// <returns>True if the application user is successfully created; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">user is null, or password is null or empty.</exception>
        /// <exception cref="Exceptions.IdentityException">Error creating the application user.</exception>
        Task<IApplicationUser> CreateAsync(IApplicationUser user, string password);

        /// <summary>
        /// Generate an email confirmation token for the specified user.
        /// </summary>
        /// <param name="user">Application user to generate an email confirmation token for.</param>
        /// <returns>An email confirmation token.</returns>
        /// <exception cref="System.ArgumentNullException">user is null.</exception>
        Task<string> GenerateEmailConfirmationTokenAsync(IApplicationUser user);

        /// <summary>
        /// Generate a two factor authentication token for the specified user.
        /// </summary>
        /// <param name="user">Application user the token is for.</param>
        /// <param name="tokenProvider">Provider which will generate the token (as defined in the application start-up configuration).</param>
        /// <returns>A two factor authentication token for the user.</returns>
        /// <exception cref="System.ArgumentNullException">user is null, or tokenProvider is null or empty.</exception>
        Task<string> GenerateTwoFactorTokenAsync(IApplicationUser user, string tokenProvider);

        /// <summary>
        /// Retrieve an application user with the specified unique identifier.
        /// </summary>
        /// <param name="userId">Unique identifier for the application user.</param>
        /// <returns>Application user matching the specified unique identifier if it exists; null otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">userId is null or empty.</exception>
        Task<IApplicationUser> RetrieveAsync(string userId);

        /// <summary>
        /// Retrieve the application user associated with the specified email address.
        /// </summary>
        /// <param name="email">Email address of the user.</param>
        /// <returns>Application user if found; null otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">email is null or empty.</exception>
        Task<IApplicationUser> RetrieveByEmailAsync(string email);

        /// <summary>
        /// Retrieve an application user by username.
        /// </summary>
        /// <param name="username">Username to search for.</param>
        /// <returns>Application user matching the specified username if found; null otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">username is null or empty.</exception>
        Task<IApplicationUser> RetrieveByNameAsync(string username);

        /// <summary>
        /// Sign in the specified user.
        /// </summary>
        /// <param name="user">User to sign-in.</param>
        /// <param name="password">Password to attempt sign-in with.</param>
        /// <param name="isPersistent">Flag indicating whether the sign-in cookie should persist after the browser is closed.</param>
        /// <param name="lockoutOnFailure">Flag indicating if the user account should be locked if the sign-in fails.</param>
        /// <returns>Task object representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">user is null, or password is null or empty.</exception>
        /// <exception cref="Exceptions.IdentityException">User sign-in failed due to an unknown reason.</exception>
        /// <exception cref="Exceptions.LockedOutException">Sign-in failed because user is locked out.</exception>
        /// <exception cref="Exceptions.NotAllowedException">Sign-in failed because user is not allowed to sign-in.</exception>
        /// <exception cref="Exceptions.TwoFactorRequiredException">Sign-in failed because it requires two-factor authentication.</exception>
        Task SignInAsync(
            IApplicationUser user,
            string password,
            bool isPersistent = false,
            bool lockoutOnFailure = true);

        /// <summary>
        /// Update the specified user.
        /// </summary>
        /// <param name="user">Application user to update.</param>
        /// <returns>Task object representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">user is null.</exception>
        /// <exception cref="Exceptions.IdentityException">User update failed due to an unknown reason.</exception>
        Task UpdateAsync(IApplicationUser user);
    }
}