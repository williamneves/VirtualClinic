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

                if (userInDb.userType == "provider")
                {
                    return RedirectToAction("Index", "Home");
                }

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

                return RedirectToAction("Index", "Home");
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

        // Show Edit Patient Profile Page
        [HttpGet("edit/patientprofile")]
        public IActionResult EditPatientProfile()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["AuthError"] = "You must be logged in to view this page";
                return RedirectToAction("Index", "Home");
            }

            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

            ViewBag.User = userInDb;

            
            // Patient Medications
            ViewBag.PatientInfo = dbContext.Patients
                                    .Include(p => p.Medications)
                                    .FirstOrDefault(p => p.UserId == userInDb.UserId);

            // Patient Allergies
            ViewBag.PatientAllergies = dbContext.Patients
                                        .Include(p => p.Allergies)
                                        .FirstOrDefault(p => p.UserId == userInDb.UserId);
    
            // Patient MedHx
            ViewBag.PatientMedHx = dbContext.Patients
                                    .Include(p => p.MedicalHistory)
                                    .FirstOrDefault(p => p.UserId == userInDb.UserId);


            return View();
        }

        // Edit Patient Profile Information
        [HttpPost("edit/patientinfo/")]
        public IActionResult EditPatientInfo(User UpdatedUser)
        {
            User OldPatient = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

            OldPatient.PreferredName = UpdatedUser.PreferredName;
            OldPatient.Pronouns = UpdatedUser.Pronouns;
            OldPatient.Email = UpdatedUser.Email;
             // Initialize hasher object
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            // Hash password
            OldPatient.Password = Hasher.HashPassword(UpdatedUser, UpdatedUser.Password);
            OldPatient.StreetAddress = UpdatedUser.StreetAddress;
            OldPatient.City = UpdatedUser.City;
            OldPatient.State = UpdatedUser.State;
            OldPatient.Zipcode = UpdatedUser.Zipcode;
            OldPatient.PhoneNumber = UpdatedUser.PhoneNumber;
            
            dbContext.SaveChanges();

            return RedirectToAction("Welcome", "Home");
        }
    
        // Get the Edit Page for Medical Info
        [HttpGet("edit/medicalinfo")]
        public IActionResult UpdateMedicalInfo()
        {
            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            
            ViewBag.UserLoggedIn = userInDb;

            // Patient Medications
            ViewBag.PatientInfo = dbContext.Patients
                                    .Include(p => p.Medications)
                                    .FirstOrDefault(p => p.UserId == userInDb.UserId);

            // Patient Allergies
            ViewBag.PatientAllergies = dbContext.Patients
                                        .Include(p => p.Allergies)
                                        .FirstOrDefault(p => p.UserId == userInDb.UserId);
    
            // Patient MedHx
            ViewBag.PatientMedHx = dbContext.Patients
                                    .Include(p => p.MedicalHistory)
                                    .FirstOrDefault(p => p.UserId == userInDb.UserId);

            return View();
        }

        // Add medications
        [HttpPost("addmed")]

        public IActionResult AddMed(Medication NewMed)
        {
            NewMed.PatientId = (int) HttpContext.Session.GetInt32("UserId");

            dbContext.Add(NewMed);
            dbContext.SaveChanges();
            return RedirectToAction("UpdateMedicalInfo");

        }

        // Add Allergy
        [HttpPost("addallergy")]

        public IActionResult AddAllergy(Allergy newAllergy)
        {
            newAllergy.PatientId = (int) HttpContext.Session.GetInt32("UserId");

            dbContext.Add(newAllergy);
            dbContext.SaveChanges();
            return RedirectToAction("UpdateMedicalInfo");

        }
        
        // Add Medical History
        [HttpPost("addmedhx")]

        public IActionResult AddMedHx(MedicalHistory newMedHx)
        {
            newMedHx.PatientId = (int) HttpContext.Session.GetInt32("UserId");

            dbContext.Add(newMedHx);
            dbContext.SaveChanges();
            return RedirectToAction("UpdateMedicalInfo");

        }

        // Remove Medication
        [HttpGet("remove/medication/{id}")]
        public IActionResult RemoveMed(int id)
        {
            Medication MedToRemove = dbContext.Medications.FirstOrDefault(d => d.MedicationId == id);
            dbContext.Medications.Remove(MedToRemove);
            dbContext.SaveChanges();
            return RedirectToAction("UpdateMedicalInfo");
        }

        // Remove Allergy
        [HttpGet("remove/allergy/{id}")]
        public IActionResult RemoveAllergy(int id)
        {
            Allergy AllergyToRemove = dbContext.Allergies.FirstOrDefault(d => d.AllergyId == id);
            dbContext.Allergies.Remove(AllergyToRemove);
            dbContext.SaveChanges();
            return RedirectToAction("UpdateMedicalInfo");
        }

        // Remove MedicalHistory
        [HttpGet("remove/medicalhx/{id}")]
        public IActionResult RemoveMedicalHistory(int id)
        {
            MedicalHistory MedicalHistoryToRemove = dbContext.MedicalHistories.FirstOrDefault(d => d.MedicalHistoryId == id);
            dbContext.MedicalHistories.Remove(MedicalHistoryToRemove);
            dbContext.SaveChanges();
            return RedirectToAction("UpdateMedicalInfo");
        }
    }
}