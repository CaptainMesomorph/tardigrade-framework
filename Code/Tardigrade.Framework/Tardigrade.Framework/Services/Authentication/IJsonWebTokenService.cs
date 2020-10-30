namespace Tardigrade.Framework.Services.Authentication
{
    /// <summary>
    /// Service interface for an authentication scheme based upon JSON Web Tokens (JWT).
    /// </summary>
    /// <typeparam name="T">Type that the JSON Web Token will be based upon.</typeparam>
    public interface IJsonWebTokenService<in T>
    {
        /// <summary>
        /// Generate a JSON Web Token for the given model.
        /// </summary>
        /// <param name="model">Model that the JSON Web Token will be based upon.</param>
        /// <returns>A JSON Web Token string.</returns>
        string GenerateToken(T model);
    }
}