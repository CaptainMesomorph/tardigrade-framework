using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Tardigrade.Framework.AspNetCore.Models.Identity;

namespace Tardigrade.Framework.AspNetCore.Services.Identity
{
    /// <summary>
    /// ASP.NET Core Identity user manager configured for this application.
    /// </summary>
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        /// <summary>
        /// <see cref="UserManager{TUser}.UserManager(IUserStore{TUser}, IOptions{IdentityOptions}, IPasswordHasher{TUser}, IEnumerable{IUserValidator{TUser}}, IEnumerable{IPasswordValidator{TUser}}, ILookupNormalizer, IdentityErrorDescriber, IServiceProvider, ILogger{UserManager{TUser}})"/>
        /// </summary>
        public ApplicationUserManager(
            IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}