using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SuszkowBlog.Views.Manage;

namespace SuszkowBlog.Controllers
{
    //[Authorize(Roles ="Owner")]
    public class ManageController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManageController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ManageRole()
        {
            var roles = _roleManager.Roles.ToList();
            ManageRoleModel model = new ManageRoleModel
            {
                Roles = roles
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(ManageRoleModel roleModel)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = roleModel.RoleName
                };
                IdentityResult result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ManageRole", "Manage");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(roleModel);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveRole(string id)
        { 
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id);
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ManageRole", "Manage");
                }

            }
            return RedirectToAction("Index", "Manage");
        }
        
    }
}
