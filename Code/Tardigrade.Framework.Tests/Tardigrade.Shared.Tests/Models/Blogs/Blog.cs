using System;
using System.Collections.Generic;

namespace Tardigrade.Shared.Tests.Models.Blogs
{
    public class Blog : BaseModel
    {
        public string Name { get; set; }
        public int Rating { get; set; }
        public string Url { get; set; }

        public Guid? OwnerId { get; set; }
        public Person Owner { get; set; }

        public IList<Post> Posts { get; set; }
    }
}