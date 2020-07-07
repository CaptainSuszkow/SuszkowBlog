﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .OrderByDescending(p=>p.CreateOn.Year)
                .ThenByDescending(p=>p.CreateOn.Month)
                .ToListAsync();
            posts.Reverse();
            return View(posts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddComment(int id)
        {
            var post = _context.Posts.First(p => p.ID == id);
            ViewBag.Title = post.Title;
            ViewBag.ID = post.ID;

            return View();
        }
        //[HttpPost]
        //[Authorize(Roles = "User")]
        //public async Task<IActionResult> AddComment(int id, Comment comment)
        //{
            
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
