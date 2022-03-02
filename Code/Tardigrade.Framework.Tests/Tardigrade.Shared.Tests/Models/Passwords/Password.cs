using System;
using Tardigrade.Framework.Models.Domain;

namespace Tardigrade.Shared.Tests.Models.Passwords
{
    public class Password : IHasUniqueIdentifier<Guid>
    {
        public Guid Id { get; set; }
        public string Word { get; set; }
    }
}