using System.Collections.Generic;

namespace Tardigrade.Shared.Tests.Models.Blogs
{
    public class Person : BaseModel
    {
        public string Name { get; set; }

        public Blog OwnedBlog { get; set; }
        //public IList<Post> Posts { get; set; }
    }
}