using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tardigrade.Framework.AspNetCore.Models.Identity;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Services.Identity;

namespace Tardigrade.Framework.AspNetCore.Services.Identity
{
    /// <summary>
    /// <see cref="IApplicationUserManager{T}"/>
    /// </summary>
    public class ApplicationUserManager : IApplicationUserManager<ApplicationUser>
    {
        private readonly IMapper mapper;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="mapper">Object mapper.</param>
        /// <param name="signInManager">Service for managing application user sign-in.</param>
        /// <param name="userManager">Service for managing an application user.</param>
        public ApplicationUserManager(
            IMapper mapper,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <summary>
        /// <see cref="IApplicationUserManager{T}.CheckPasswordAsync(T, string)"/>
        /// </summary>
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            return await userManager.CheckPasswordAsync(user, password);
        }

        /// <summary>
        /// <see cref="IApplicationUserManager{T}.CreateAsync(T, string)"/>
        /// </summary>
        public async Task<ApplicationUser> CreateAsync(ApplicationUser user, string password)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            IdentityResult result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                IEnumerable<Framework.Models.Errors.IdentityError> errors =
                    mapper.Map<IEnumerable<Framework.Models.Errors.IdentityError>>(result.Errors);

                throw new IdentityException($"Create user failed; unable to create user with email {user.Email}.", errors);
            }

            return user;
        }

        /// <summary>
        /// <see cref="IApplicationUserManager{T}.GenerateEmailConfirmationTokenAsync(T)"/>
        /// </summary>
        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        /// <summary>
        /// <see cref="IApplicationUserManager{T}.GenerateTwoFactorTokenAsync(T, string)"/>
        /// </summary>
        public async Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user, string tokenProvider)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(tokenProvider)) throw new ArgumentNullException(nameof(tokenProvider));

            return await userManager.GenerateTwoFactorTokenAsync(user, tokenProvider);
        }

        /// <summary>
        /// <see cref="IApplicationUserManager{T}.RetrieveAsync(string)"/>
        /// </summary>
        public async Task<ApplicationUser> RetrieveAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

            return await userManager.FindByIdAsync(userId);
        }

        /// <summary>
        /// <see cref="IApplicationUserManager{T}.RetrieveByEmailAsync(string)"/>
        /// </summary>
        public async Task<ApplicationUser> RetrieveByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));

            return await userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// <see cref="IApplicationUserManager{T}.RetrieveByNameAsync(string)"/>
        /// </summary>
        public async Task<ApplicationUser> RetrieveByNameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));

            return await userManager.FindByNameAsync(username);
        }

        /// <summary>
        /// <see cref="IApplicationUserManager{T}.SignInAsync(T, string, bool, bool)"/>
        /// </summary>
        public async Task SignInAsync(
            ApplicationUser user,
            string password,
            bool isPersistent = false,
            bool lockoutOnFailure = true)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            SignInResult result = await signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    throw new LockedOutException($"Sign-in failed; user {user.UserName} has been locked out.");
                }
                else if (result.IsNotAllowed)
                {
                    throw new NotAllowedException($"Sign-in failed; sign-in for user {user.UserName} is not allowed as email has yet to be confirmed.");
                }
                else if (result.RequiresTwoFactor)
                {
                    throw new TwoFactorRequiredException($"Sign-in failed; Two-Factor Authentication required for user {user.UserName}.");
                }
                else
                {
                    throw new IdentityException($"Sign-in failed; for reasons unknown, not able to sign-in user {user.Email}.");
                }
            }
        }

        /// <summary>
        /// <see cref="IApplicationUserManager{T}.UpdateAsync(T)"/>
        /// </summary>
        public async Task UpdateAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            IdentityResult result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                IEnumerable<Framework.Models.Errors.IdentityError> errors =
                    mapper.Map<IEnumerable<Framework.Models.Errors.IdentityError>>(result.Errors);

                throw new IdentityException($"Update user failed; unable to update user with email {user.Email}.", errors);
            }
        }
    }
}