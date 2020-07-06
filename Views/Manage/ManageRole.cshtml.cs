using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SuszkowBlog.Views.Manage
{
    public class ManageRoleModel
    {
        [Required(ErrorMessage = "Role Name is required")]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
        public ManageRoleModel()
        {
            Users = new List<string>();
            Roles = new List<IdentityRole>();
        }

        public string Id { get; set; }
        public List<string> Users { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}
