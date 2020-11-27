using System;
using System.Collections.Generic;
using Tardigrade.Framework.Models.Domain;

namespace Tardigrade.Framework.Tests.Models
{
    internal class Credential : IHasUniqueIdentifier<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual CredentialIssuer Issuer { get; set; }
        public virtual ICollection<CredentialIssuer> Issuers { get; set; }
    }
}