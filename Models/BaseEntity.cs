using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuszkowBlog.Models
{
    public class BaseEntity
    {
        public DateTime CreateOn { get; set; }
        public DateTime? UpdateOn { get; set; }
    }
}
