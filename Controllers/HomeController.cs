using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PusherServer;
using SuszkowBlog.Areas.Identity.Data;
using SuszkowBlog.Data;
using SuszkowBlog.Models;

namespace SuszkowBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataDbContext _context;
  
        public HomeController(ILogger<HomeController> logger, DataDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            var posts = _context.Posts
                .OrderByDescending(p => p.CreateOn.Year)
                .ThenByDescending(p => p.CreateOn.Month)
                .AsNoTracking();
            int pageSize = 4;
            return View(await PaginatedList<Post>.CreateAsync(posts, pageNumber ?? 1, pageSize));
        }
        public async Task<IActionResult> ShowPost(int id)
        {
            return View((Post) await _context.Posts.FindAsync(id));
        }

        public IActionResult About()
        {
            return View();
        }
        public ActionResult Comments(int? id)
        {
            var comments = _context.Comments.Where(x => x.PostID == id).ToArray();
            return Json(comments);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> AddComment(string content, int postID)
        {
            var data = new Comment();
            data.UserID = _userManager.GetUserId(User);
            data.Content = content;
            data.PostID = postID;
            _context.Comments.Add(data);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");//, new { id = postID });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
