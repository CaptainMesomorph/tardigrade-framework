using System;

namespace Tardigrade.Shared.Tests.Models
{
    public class CredentialIssuer : BaseModel
    {
        public string Name { get; set; }

        // Inverse navigation properties.

        public Credential Credential { get; set; }
        public Guid CredentialId { get; set; }
    }
}