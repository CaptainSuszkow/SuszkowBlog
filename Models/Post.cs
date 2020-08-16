using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SuszkowBlog.Models
{
    public class Post : BaseEntity
    {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
    