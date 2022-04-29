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
using Microsoft.AspNetCore.Hosting;
using System.IO;


namespace VirtualClinic.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private MyContext dbContext;
        private IWebHostEnvironment hostEnvironment;

        public AuthController(ILogger<AuthController> logger, MyContext context, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            dbContext = context;
            this.hostEnvironment = hostEnvironment;
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

                if (userInDb.userType == "provider")
                {
                    return RedirectToAction("ProviderDashboard", "Provider");
                }

                return RedirectToAction("PatientDashboard", "Patient");
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
        [HttpGet("registerprovider")]
        public IActionResult RegisterProvider()
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
        
        [HttpPost("RegisterUserProvider")]
        public IActionResult RegisterUserProvider(User newUser)
        {
            // Set userType to Provider
            newUser.userType = "provider";
            
            // Check initial ModelState
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                if (dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    // Email already exists
                    TempData["LoginError"] = "emailinuse";
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("RegisterProvider");
                }

                // Initialize hasher object
                PasswordHasher<User> Hasher = new PasswordHasher<User>();

                // Hash password
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

                // Save new user to database
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                
                // Attach new patient to the user
                Provider newProvider = new Provider();
                newProvider.UserId = newUser.UserId;
                dbContext.Add(newProvider);
                dbContext.SaveChanges();

                // Save new user to session
                HttpContext.Session.SetInt32("UserId", newUser.UserId);

                ViewBag.UserLoggedIn = newUser;

                return RedirectToAction("ProviderDashboard", "Provider");
            }

            // Not Valid ModelState
            TempData["LoginError"] = "modelstateinvalid";
            return View("RegisterProvider");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        // Patient Profile Picture
        [HttpPost]
        public async Task<IActionResult> UploadPatientPic(User UpdatedUser)
        {

            string wwwroot = hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(UpdatedUser.ImageFile.FileName);
            string extension = Path.GetExtension(UpdatedUser.ImageFile.FileName);
            UpdatedUser.ImageProfile = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwroot + "/imgs/profileimg/", fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await UpdatedUser.ImageFile.CopyToAsync(fileStream);
            }
            
            var OldPatient = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            OldPatient.PreferredName = UpdatedUser.PreferredName;
            OldPatient.Pronouns = UpdatedUser.Pronouns;
            OldPatient.Email = OldPatient.Email;
            OldPatient.Password = OldPatient.Password;
             // Initialize hasher object
            // PasswordHasher<User> Hasher = new PasswordHasher<User>();
            // // Hash password
            // OldPatient.Password = Hasher.HashPassword(UpdatedUser, UpdatedUser.Password);
            OldPatient.StreetAddress = UpdatedUser.StreetAddress;
            OldPatient.ImageProfile = UpdatedUser.ImageProfile;
            OldPatient.City = UpdatedUser.City;
            OldPatient.State = UpdatedUser.State;
            OldPatient.Zipcode = UpdatedUser.Zipcode;
            OldPatient.PhoneNumber = UpdatedUser.PhoneNumber;
            
            await dbContext.SaveChangesAsync();

            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

            if(userInDb.userType == "patient")
            {
                return RedirectToAction("PatientDashboard");
            }

            return RedirectToAction("ProviderDashboard");
        }

    }
}