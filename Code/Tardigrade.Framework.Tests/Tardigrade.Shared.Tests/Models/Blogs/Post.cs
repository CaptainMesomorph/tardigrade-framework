using System;

namespace Tardigrade.Shared.Tests.Models.Blogs
{
    public class Post : BaseModel
    {
        public string Content { get; set; }
        public string Title { get; set; }

        public Person Author { get; set; }
        public Guid? AuthorId { get; set; }

        public Blog Blog { get; set; }
        public Guid? BlogId { get; set; }
    }
}