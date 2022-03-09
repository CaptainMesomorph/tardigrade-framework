using Tardigrade.Framework.Models.Domain;

namespace Tardigrade.Framework.RestSharp.Tests.Models;

/// <summary>
/// Users model for the API
/// </summary>
public class User : IHasUniqueIdentifier<string>
{
    /// <summary>
    /// The created date of the user
    /// </summary>
    public string? CreatedDateUtc { get; set; }

    /// <summary>
    /// Date of birth of the user
    /// </summary>
    public string? DateOfBirth { get; set; }

    /// <summary>
    /// Email address of the user
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// First name of the user
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Unique Identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Last name of the user
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// First name of the user
    /// </summary>
    public string? MiddleNames { get; set; }
}