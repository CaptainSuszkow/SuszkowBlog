using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SuszkowBlog.Models
{
    public class Comment : BaseEntity
    {
        public string ID { get; set; }
        public string UserID { get; set; }

        [Required]
        [StringLength(300)]
        public string Content { get; set; }
        [Required]
        public int PostID { get; set; }
    }
}
