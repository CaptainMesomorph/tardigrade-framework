using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tardigrade.Framework.AspNet.Models.Identity;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Models.Errors;
using Tardigrade.Framework.Services.Identity;

namespace Tardigrade.Framework.AspNet.Services.Identity
{
    /// <summary>
    /// <see cref="IIdentityUserManager{T}"/>
    /// </summary>
    public class IdentityUserManager : IIdentityUserManager<ApplicationUser>, IDisposable
    {
        private bool disposedValue = false; // To detect redundant calls
        private SignInManager<ApplicationUser, string> signInManager;
        private UserManager<ApplicationUser> userManager;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="signInManager">Service for managing application user sign-in.</param>
        /// <param name="userManager">Service for managing an application user.</param>
        public IdentityUserManager(
            SignInManager<ApplicationUser, string> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.AddPasswordAsync(T, string)"/>
        /// </summary>
        public async Task AddPasswordAsync(ApplicationUser user, string password)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            IdentityResult result = await userManager.AddPasswordAsync(user.Id, password);

            if (!result.Succeeded)
            {
                IEnumerable<IdentityError> errors = result.Errors.Select(e => new IdentityError("AddPasswordFailed", e));

                throw new IdentityException($"Add password failed; unable to add password for {user.UserName}.", errors);
            }
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.CheckPasswordAsync(T, string)"/>
        /// </summary>
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            return await userManager.CheckPasswordAsync(user, password);
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.ConfirmEmailAsync(T, string)"/>
        /// </summary>
        public async Task ConfirmEmailAsync(ApplicationUser user, string token)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));

            IdentityResult result = await userManager.ConfirmEmailAsync(user.Id, token);

            if (!result.Succeeded)
            {
                IEnumerable<IdentityError> errors = result.Errors.Select(e => new IdentityError("ConfirmEmailFailed", e));

                throw new IdentityException($"Confirm email failed; unable to confirm email for {user.UserName}.", errors);
            }
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.CreateAsync(T, string)"/>
        /// </summary>
        public async Task<ApplicationUser> CreateAsync(ApplicationUser user, string password)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            IdentityResult result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                IEnumerable<IdentityError> errors = result.Errors.Select(e => new IdentityError("CreateFailed", e));

                throw new IdentityException($"Create user failed; unable to create user with email {user.Email}.", errors);
            }

            return user;
        }

        /// <summary>
        /// <see cref="IDisposable.Dispose()"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Cleanly dispose this service.
        /// </summary>
        /// <param name="disposing">True to dispose; false otherwise.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (userManager != null)
                    {
                        userManager.Dispose();
                        userManager = null;
                    }

                    if (signInManager != null)
                    {
                        signInManager.Dispose();
                        signInManager = null;
                    }
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.GenerateEmailConfirmationTokenAsync(T)"/>
        /// </summary>
        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return await userManager.GenerateEmailConfirmationTokenAsync(user.Id);
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.GeneratePasswordResetTokenAsync(T)"/>
        /// </summary>
        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return await userManager.GeneratePasswordResetTokenAsync(user.Id);
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.GenerateTwoFactorTokenAsync(T, string)"/>
        /// </summary>
        public async Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user, string tokenProvider)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(tokenProvider)) throw new ArgumentNullException(nameof(tokenProvider));

            return await userManager.GenerateTwoFactorTokenAsync(user.Id, tokenProvider);
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.GetAccessFailedCountAsync(T)"/>
        /// </summary>
        public async Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return await userManager.GetAccessFailedCountAsync(user.Id);
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.IsEmailConfirmedAsync(T)"/>
        /// </summary>
        public async Task<bool> IsEmailConfirmedAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return await userManager.IsEmailConfirmedAsync(user.Id);
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.IsPhoneNumberConfirmedAsync(T)"/>
        /// </summary>
        public async Task<bool> IsPhoneNumberConfirmedAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return await userManager.IsPhoneNumberConfirmedAsync(user.Id);
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.RemovePasswordAsync(T)"/>
        /// </summary>
        public async Task RemovePasswordAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            IdentityResult result = await userManager.RemovePasswordAsync(user.Id);

            if (!result.Succeeded)
            {
                IEnumerable<IdentityError> errors = result.Errors.Select(e => new IdentityError("RemovePasswordFailed", e));

                throw new IdentityException($"Remove password failed; unable to remove password for {user.UserName}.", errors);
            }
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.ResetPasswordAsync(T, string, string)"/>
        /// </summary>
        public async Task ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));

            if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentNullException(nameof(newPassword));

            IdentityResult result = await userManager.ResetPasswordAsync(user.Id, token, newPassword);

            if (!result.Succeeded)
            {
                IEnumerable<IdentityError> errors = result.Errors.Select(e => new IdentityError("ResetPasswordFailed", e));

                throw new IdentityException($"Reset password failed; unable to reset password for {user.UserName}.", errors);
            }
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.RetrieveAsync(string)"/>
        /// </summary>
        public async Task<ApplicationUser> RetrieveAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

            return await userManager.FindByIdAsync(userId);
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.RetrieveByEmailAsync(string)"/>
        /// </summary>
        public async Task<ApplicationUser> RetrieveByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));

            return await userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.RetrieveByNameAsync(string)"/>
        /// </summary>
        public async Task<ApplicationUser> RetrieveByNameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));

            return await userManager.FindByNameAsync(username);
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.SignInAsync(T, bool, string)"/>
        /// </summary>
        /// <param name="authenticationMethod">Not supported.</param>
        public async Task SignInAsync(
            ApplicationUser user,
            bool isPersistent = false,
            string authenticationMethod = null)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            await signInManager.SignInAsync(user, isPersistent, false);
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.SignInAsync(T, string, bool, bool)"/>
        /// </summary>
        /// <exception cref="NotAllowedException">Not supported.</exception>
        public async Task SignInAsync(
            ApplicationUser user,
            string password,
            bool isPersistent = false,
            bool lockoutOnFailure = true)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            SignInStatus result = await signInManager.PasswordSignInAsync(
                user.UserName,
                password,
                isPersistent,
                lockoutOnFailure);

            switch (result)
            {
                case SignInStatus.LockedOut:

                    throw new LockedOutException($"Sign-in failed; user {user.UserName} has been locked out.");

                case SignInStatus.RequiresVerification:

                    throw new TwoFactorRequiredException($"Sign-in failed; Two-Factor Authentication required for user {user.UserName}.");

                case SignInStatus.Failure:

                    throw new IdentityException($"Sign-in failed; for reasons unknown, not able to sign-in user {user.UserName}.");

                case SignInStatus.Success:
                default:
                    break;
            }
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.SignOutAsync()"/>
        /// </summary>
        /// <exception cref="NotImplementedException">Not currently implemented.</exception>
        public Task SignOutAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.UpdateAsync(T)"/>
        /// </summary>
        public async Task UpdateAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            IdentityResult result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                IEnumerable<IdentityError> errors = result.Errors.Select(e => new IdentityError("UpateFailed", e));

                throw new IdentityException($"Update user failed; unable to update user {user.UserName}.", errors);
            }
        }

        /// <summary>
        /// <see cref="IIdentityUserManager{T}.UpdateSecurityStampAsync(T)"/>
        /// </summary>
        public async Task UpdateSecurityStampAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            IdentityResult result = await userManager.UpdateSecurityStampAsync(user.Id);

            if (!result.Succeeded)
            {
                IEnumerable<IdentityError> errors = result.Errors.Select(e => new IdentityError("UpdateSecurityStampFailed", e));

                throw new IdentityException($"Update security stamp failed; unable to update security stamp for {user.UserName}.", errors);
            }
        }
    }
}