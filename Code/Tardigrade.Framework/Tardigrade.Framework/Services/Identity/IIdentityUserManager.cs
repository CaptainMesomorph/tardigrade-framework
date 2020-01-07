using System.Threading.Tasks;

namespace Tardigrade.Framework.Services.Identity
{
    /// <summary>
    /// Interface that defines operations associated with application users.
    /// </summary>
    /// <typeparam name="TUser">Type associated with the application user definition.</typeparam>
    public interface IIdentityUserManager<TUser>
    {
        /// <summary>
        /// Add the password to the specified user only if the user does not already have a password.
        /// </summary>
        /// <param name="user">Application user whose password should be set.</param>
        /// <param name="password">Password to set.</param>
        /// <returns>Task object representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">user is null, or password is null or empty.</exception>
        /// <exception cref="Exceptions.IdentityException">Add password failed due to an unknown reason.</exception>
        Task AddPasswordAsync(TUser user, string password);

        /// <summary>
        /// Check whether the given password is valid for the specified application user.
        /// </summary>
        /// <param name="user">Application user whose password should be validated.</param>
        /// <param name="password">Password to validate.</param>
        /// <returns>True if the specified password matches the one stored; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">user is null, or password is null or empty.</exception>
        Task<bool> CheckPasswordAsync(TUser user, string password);

        /// <summary>
        /// Validate that an email confirmation token matches the specified user.
        /// </summary>
        /// <param name="user">Application user to validate the token against.</param>
        /// <param name="token">Email confirmation token to validate.</param>
        /// <returns>Task object representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">user is null, or token is null or empty.</exception>
        /// <exception cref="Exceptions.IdentityException">Confirm email failed due to an unknown reason.</exception>
        Task ConfirmEmailAsync(TUser user, string token);

        /// <summary>
        /// Create the specified application user with the password provided.
        /// </summary>
        /// <param name="user">Application user to create.</param>
        /// <param name="password">Password for the application user.</param>
        /// <returns>Application user created (including allocated unique identifier).</returns>
        /// <exception cref="System.ArgumentNullException">user is null, or password is null or empty.</exception>
        /// <exception cref="Exceptions.IdentityException">Error creating the application user.</exception>
        Task<TUser> CreateAsync(TUser user, string password);

        /// <summary>
        /// Generate an email confirmation token for the specified user.
        /// </summary>
        /// <param name="user">Application user to generate an email confirmation token for.</param>
        /// <returns>An email confirmation token.</returns>
        /// <exception cref="System.ArgumentNullException">user is null.</exception>
        Task<string> GenerateEmailConfirmationTokenAsync(TUser user);

        /// <summary>
        /// Generate a password reset token for the specified user.
        /// </summary>
        /// <param name="user">Application user to generate a password reset token for.</param>
        /// <returns>Password reset token for the specified user.</returns>
        /// <exception cref="System.ArgumentNullException">user is null.</exception>
        Task<string> GeneratePasswordResetTokenAsync(TUser user);

        /// <summary>
        /// Generate a two factor authentication token for the specified user.
        /// </summary>
        /// <param name="user">Application user the token is for.</param>
        /// <param name="tokenProvider">Provider which will generate the token (as defined in the application start-up configuration).</param>
        /// <returns>A two factor authentication token for the user.</returns>
        /// <exception cref="System.ArgumentNullException">user is null, or tokenProvider is null or empty.</exception>
        Task<string> GenerateTwoFactorTokenAsync(TUser user, string tokenProvider);

        /// <summary>
        /// Retrieve the current number of failed accesses for the given user.
        /// </summary>
        /// <param name="user">Application user whose access failed count should be retrieved for.</param>
        /// <returns>Current failed access count for the user.</returns>
        /// <exception cref="System.ArgumentNullException">user is null.</exception>
        Task<int> GetAccessFailedCountAsync(TUser user);

        /// <summary>
        /// Check whether the email address for the specified user has been verified.
        /// </summary>
        /// <param name="user">Application user whose email confirmation status should be returned.</param>
        /// <returns>True if the email address is verified; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">user is null.</exception>
        Task<bool> IsEmailConfirmedAsync(TUser user);

        /// <summary>
        /// Check whether the user is in the specified role.
        /// </summary>
        /// <param name="user">Application user to check.</param>
        /// <param name="role">Role to check.</param>
        /// <returns>True if the user is in the specified role; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">user is null, and/or role is null or empty.</exception>
        Task<bool> IsInRoleAsync(TUser user, string role);

        /// <summary>
        /// Check whether the specified user's telephone number has been confirmed.
        /// </summary>
        /// <param name="user">Application user whose phone number confirmation status should be returned.</param>
        /// <returns>True if the specified user has a confirmed telephone number; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">user is null.</exception>
        Task<bool> IsPhoneNumberConfirmedAsync(TUser user);

        /// <summary>
        /// Remove an application user's password.
        /// </summary>
        /// <param name="user">Application user whose password should be removed.</param>
        /// <returns>Task object representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">user is null.</exception>
        /// <exception cref="Exceptions.IdentityException">Remove password failed due to an unknown reason.</exception>
        Task RemovePasswordAsync(TUser user);

        /// <summary>
        /// Reset the application user's password to the specified new password after validating the given password
        /// reset token.
        /// </summary>
        /// <param name="user">Application user whose password should be reset.</param>
        /// <param name="token">Password reset token to verify.</param>
        /// <param name="newPassword">New password to set if reset token verification succeeds.</param>
        /// <returns>Task object representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">user is null, or token null or empty, or newPassword null or empty.</exception>
        /// <exception cref="Exceptions.IdentityException">Reset password failed due to an unknown reason.</exception>
        Task ResetPasswordAsync(TUser user, string token, string newPassword);

        /// <summary>
        /// Retrieve an application user with the specified unique identifier.
        /// </summary>
        /// <param name="userId">Unique identifier for the application user.</param>
        /// <returns>Application user matching the specified unique identifier if it exists; null otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">userId is null or empty.</exception>
        Task<TUser> RetrieveAsync(string userId);

        /// <summary>
        /// Retrieve the application user associated with the specified email address.
        /// </summary>
        /// <param name="email">Email address of the user.</param>
        /// <returns>Application user if found; null otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">email is null or empty.</exception>
        Task<TUser> RetrieveByEmailAsync(string email);

        /// <summary>
        /// Retrieve an application user by username.
        /// </summary>
        /// <param name="username">Username to search for.</param>
        /// <returns>Application user matching the specified username if found; null otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">username is null or empty.</exception>
        Task<TUser> RetrieveByNameAsync(string username);

        /// <summary>
        /// Sign in the specified user.
        /// </summary>
        /// <param name="user">Application user to sign-in.</param>
        /// <param name="isPersistent">Flag indicating whether the sign-in cookie should persist after the browser is closed.</param>
        /// <param name="authenticationMethod">Name of the method used to authenticate the user.</param>
        /// <returns>Task object representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">user is null.</exception>
        Task SignInAsync(TUser user, bool isPersistent = false, string authenticationMethod = null);

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
            TUser user,
            string password,
            bool isPersistent = false,
            bool lockoutOnFailure = true);

        /// <summary>
        /// Signs the current user out of the application.
        /// </summary>
        /// <returns>Task object representing the asynchronous operation.</returns>
        Task SignOutAsync();

        /// <summary>
        /// Update the specified user.
        /// </summary>
        /// <param name="user">Application user to update.</param>
        /// <returns>Task object representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">user is null.</exception>
        /// <exception cref="Exceptions.IdentityException">User update failed due to an unknown reason.</exception>
        Task UpdateAsync(TUser user);

        /// <summary>
        /// Regenerate the security stamp for the specified user.
        /// </summary>
        /// <param name="user">Application user whose security stamp should be regenerated.</param>
        /// <returns>Task object representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">user is null.</exception>
        /// <exception cref="Exceptions.IdentityException">User update failed due to an unknown reason.</exception>
        Task UpdateSecurityStampAsync(TUser user);

        /// <summary>
        /// Verify the specified two factor authentication token against the user.
        /// </summary>
        /// <param name="user">User the token applies to.</param>
        /// <param name="tokenProvider">Provider which will verify the token.</param>
        /// <param name="token">Token to verify.</param>
        /// <returns>True if the token is valid; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">user is null, or either tokenProvider or token is null or empty.</exception>
        Task<bool> VerifyTwoFactorTokenAsync(TUser user, string tokenProvider, string token);
    }
}