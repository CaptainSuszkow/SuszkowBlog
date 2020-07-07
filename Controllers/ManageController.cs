using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SuszkowBlog.Areas.Identity.Data;
using SuszkowBlog.Data;
using SuszkowBlog.Models;
using SuszkowBlog.Views.Manage;

namespace SuszkowBlog.Controllers
{
    [Authorize(Roles ="Owner")]
    public class ManageController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DataDbContext _context;

        public ManageController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, DataDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ManageRole()
        {
            ManageRoleModel model = new ManageRoleModel
            {
                Roles = _roleManager.Roles.ToList()
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

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if(role == null)
            {
                ViewBag.ErrorMesage = $"Role with Id = {id} doesn't exist";
                return View("NotFound");
            }

            var model = new ManageRoleModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach( var user in _userManager.Users.ToList())
            {
                if(await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }

        [HttpPost] 
        public async Task<IActionResult> EditRole(ManageRoleModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} doesn't exists";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ManageRole");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UsersInRoleModel>();

            foreach (var user in _userManager.Users)
            {
                var usersInRoleModel = new UsersInRoleModel
                {
                    Id = user.Id,
                    Name = user.UserName,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
                };


                model.Add(usersInRoleModel);
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UsersInRoleModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            foreach(var item in model)
            {
                var user = await _userManager.FindByIdAsync(item.Id);

                IdentityResult result = null;
                bool IsInRole = await _userManager.IsInRoleAsync(user, role.Name);

                if (item.IsSelected && !IsInRole)
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if(IsInRole && !item.IsSelected)
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (item != model.Last())
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }
        [HttpGet]
        public IActionResult ManagePost()
        {
            return View(_context.Posts.ToList());
        }
        [HttpGet]
        public IActionResult AddPost()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddPost(Post post)
        {
            await _context.Posts.AddAsync(post);
            var result = await _context.SaveChangesAsync();
            return RedirectToAction("ManagePost", "Manage");
        }
        [HttpPost]
        public async Task<IActionResult> RemovePost(int id)
        {
            var post = _context.Posts.First(p => p.ID == id);
            _context.Posts.Remove(post);
            var result = await _context.SaveChangesAsync();

            return RedirectToAction("ManagePost", "Manage");
        }
        [HttpGet]
        public IActionResult EditPost(int id)
        {
            var post = _context.Posts.First(p => p.ID == id);
            return View(post);
        }
        [HttpPost]
        public async Task<IActionResult> EditPost(Post post)
        {
            await _context.Posts.AddAsync(post);
            _context.Entry(post).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction("ManagePost", "Manage");
        }

        [HttpPost]
        [Route("Manage/RemoveComment/{postId}/{id}")]
        public async Task<IActionResult> RemoveComment(string id, int postId)
        {
            var comment = _context.Comments.First(c => c.ID == id);
            _context.Comments.Remove(comment);
            var result = await _context.SaveChangesAsync();

            return RedirectToAction("EditPost", "Manage", new { id = postId });
        }
    }
}
