using System;
using System.Collections.Generic;
using Tardigrade.Framework.Models.Domain;

namespace Tardigrade.Framework.Tests.Models
{
    internal class UserCredential : IHasUniqueIdentifier<Guid>
    {
        public DateTime CreatedDate { get; set; }
        public Guid Id { get; set; }
        public string Status { get; set; }
        public virtual Credential Credential { get; set; }
        public virtual ICollection<Credential> Credentials { get; set; }
    }
}
