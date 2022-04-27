using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VirtualClinic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace VirtualClinic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext dbContext;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            dbContext = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            
                Console.WriteLine("User Logged In: ", HttpContext.Session.GetInt32("UserId"));
            
                ViewBag.UserLoggedIn = userInDb;
            }
            
            return View();
        }

        
        [HttpGet("test")]
        public IActionResult Test()
        {
            return View();
        }

    }
}