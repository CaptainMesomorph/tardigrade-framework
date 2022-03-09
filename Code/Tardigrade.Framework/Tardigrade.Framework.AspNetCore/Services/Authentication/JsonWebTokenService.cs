using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tardigrade.Framework.AspNetCore.Models.Identity;
using Tardigrade.Framework.Services.Authentication;

namespace Tardigrade.Framework.AspNetCore.Services.Authentication;

/// <summary>
/// <see cref="IJsonWebTokenService{T}"/>
/// TODO Manage the Secret value more securely.
/// </summary>
public class JsonWebTokenService : IJsonWebTokenService<ApplicationUser>
{
    private const string Secret = "supercalifragilisticexpialidocious";

    /// <summary>
    /// Security key associated with this JSON Web Token service.
    /// </summary>
    public static SymmetricSecurityKey SecurityKey => new(Encoding.UTF8.GetBytes(Secret));

    /// <summary>
    /// <see cref="IJsonWebTokenService{T}.GenerateToken(T)"/>
    /// </summary>
    public string GenerateToken(ApplicationUser model)
    {
        var notBefore = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString();
        var expirationTime = new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString();

        // https://docs.microsoft.com/en-us/azure/active-directory/develop/id-tokens#using-claims-to-reliably-identify-a-user-subject-and-object-id
        Claim[] claims = {
            new(ClaimTypes.Name, model.UserName),
            new(ClaimTypes.NameIdentifier, model.Id),
            new(JwtRegisteredClaimNames.Email, model.Email),
            new(JwtRegisteredClaimNames.Exp, expirationTime),
            new(JwtRegisteredClaimNames.Nbf, notBefore),
            new(JwtRegisteredClaimNames.Sub, model.Id),
        };

        var credential = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        var header = new JwtHeader(credential);
        var payload = new JwtPayload(claims);
        var securityToken = new JwtSecurityToken(header, payload);
        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;
    }
}