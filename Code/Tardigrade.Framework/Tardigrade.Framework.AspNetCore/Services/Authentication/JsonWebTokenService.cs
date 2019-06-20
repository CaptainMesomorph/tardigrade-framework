using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tardigrade.Framework.AspNetCore.Models.Identity;
using Tardigrade.Framework.Services.Authentication;

namespace Tardigrade.Framework.AspNetCore.Services.Authentication
{
    /// <summary>
    /// <see cref="IJsonWebTokenService{T}"/>
    /// TODO: Manage the Secret value more securely.
    /// </summary>
    public class JsonWebTokenService : IJsonWebTokenService<ApplicationUser>
    {
        private const string Secret = "supercalifragilisticexpialidocious";

        /// <summary>
        /// Security key associated with this JSON Web Token service.
        /// </summary>
        public static SymmetricSecurityKey SecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));

        /// <summary>
        /// <see cref="IJsonWebTokenService{T}.GenerateToken(T)"/>
        /// </summary>
        public string GenerateToken(ApplicationUser model)
        {
            string notBefore = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString();
            string expirationTime = new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString();

            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, model.UserName),
                new Claim(JwtRegisteredClaimNames.Email, model.Email),
                new Claim(JwtRegisteredClaimNames.Nbf, notBefore),
                new Claim(JwtRegisteredClaimNames.Exp, expirationTime),
            };

            SigningCredentials credential = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            JwtHeader header = new JwtHeader(credential);
            JwtPayload payload = new JwtPayload(claims);
            JwtSecurityToken securityToken = new JwtSecurityToken(header, payload);
            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return token;
        }
    }
}