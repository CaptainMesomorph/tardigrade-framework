using System;
using System.Collections.Generic;
using System.Linq;

namespace Tardigrade.Shared.Tests.Models
{
    public class UserCredential : BaseModel
    {
        public Credential LatestCredential => Credentials?.OrderByDescending(o => o.CreatedDate).FirstOrDefault();
        public string Status { get; set; }

        // One-to-many navigation properties.

        public virtual ICollection<Credential> Credentials { get; set; }

        // Inverse navigation properties.

        public User User { get; set; }
        public Guid UserId { get; set; }
    }
}