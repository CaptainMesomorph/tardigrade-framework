using System;
using System.Collections.Generic;
using Tardigrade.Framework.Models.Domain;

namespace Tardigrade.Framework.Tests.Models
{
    internal class User : IHasUniqueIdentifier<Guid>
    {
        public string Email { get; set; }
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual ICollection<UserCredential> UserCredentials { get; set; }
    }
}