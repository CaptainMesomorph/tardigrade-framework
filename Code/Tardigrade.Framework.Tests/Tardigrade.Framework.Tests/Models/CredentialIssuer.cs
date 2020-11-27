using System;
using Tardigrade.Framework.Models.Domain;

namespace Tardigrade.Framework.Tests.Models
{
    internal class CredentialIssuer : IHasUniqueIdentifier<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}