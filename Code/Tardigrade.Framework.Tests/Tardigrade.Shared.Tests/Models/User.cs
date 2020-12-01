using System.Collections.Generic;

namespace Tardigrade.Shared.Tests.Models
{
    public class User : BaseModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // One-to-many navigation properties.

        public virtual ICollection<UserCredential> UserCredentials { get; set; }
    }
}