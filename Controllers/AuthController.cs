using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VirtualClinic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace VirtualClinic.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private MyContext dbContext;

        public AuthController(ILogger<AuthController> logger, MyContext context)
        {
            _logger = logger;
            dbContext = context;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("LoginUser")]
        public IActionResult LoginUser(LoginUser userSubmission)
        {
            // Check initial ModelState
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);

                if (userInDb == null)
                {
                    // If no user exists with provided email
                    ModelState.AddModelError("Email", "*User not found");
                    TempData["LoginError"] = "emailnotfound";
                    return View("Login");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

                if (result == 0)
                {
                    // password does not match
                    ModelState.AddModelError("Password", "*Password is incorrect");
                    TempData["LoginError"] = "passwordincorrect";
                    return View("Login");
                }

                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                Console.WriteLine("User Login Successful");

                ViewBag.UserLoggedIn = userInDb;

                return RedirectToAction("Welcome", "Home");
            }

            // Not Valid ModelState
            TempData["LoginError"] = "invalidform";
            return View("Login");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            TempData["Logout"] = "Logout Successful";
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet("registermodal")]
        public IActionResult RegisterModal()
        {
            return PartialView("LoginRegisterModel");
        }

        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser(User newUser)
        {
            // Check initial ModelState
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                if (dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    // Email already exists
                    TempData["LoginError"] = "emailinuse";
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Register");
                }

                // Initialize hasher object
                PasswordHasher<User> Hasher = new PasswordHasher<User>();

                // Hash password
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

                // Save new user to database
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                
                // Attach new patient to the user
                Patient newPatient = new Patient();
                newPatient.UserId = newUser.UserId;
                dbContext.Add(newPatient);
                dbContext.SaveChanges();

                // Save new user to session
                HttpContext.Session.SetInt32("UserId", newUser.UserId);

                ViewBag.UserLoggedIn = newUser;

                return RedirectToAction("Index", "Home");
            }

            // Not Valid ModelState
            TempData["LoginError"] = "modelstateinvalid";
            return View("Register");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}