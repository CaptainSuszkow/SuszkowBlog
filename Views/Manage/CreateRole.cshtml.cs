using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SuszkowBlog.Views.Manage
{
    public class CreateRoleModel
    {

        [Required]
        [Display(Name ="Role")]
        public string RoleName { get; set; }
        public void OnGet()
        {
        }
    }
}
