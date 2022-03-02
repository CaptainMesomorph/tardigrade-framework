using System;
using System.Collections.Generic;
using System.Linq;

namespace Tardigrade.Shared.Tests.Models.Credentials
{
    public class Credential : BaseModel
    {
        public CredentialIssuer LatestIssuer => Issuers?.OrderByDescending(o => o.CreatedDate).FirstOrDefault();
        public string Name { get; set; }

        // One-to-many navigation properties.

        public virtual ICollection<CredentialIssuer> Issuers { get; set; }

        // Inverse navigation properties.

        public UserCredential UserCredential { get; set; }
        public Guid UserCredentialId { get; set; }
    }
}