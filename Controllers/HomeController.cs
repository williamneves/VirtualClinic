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
            // var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

            // ViewBag.UserLoggedIn = userInDb;
            return View();
        }

        [HttpGet("welcome")]
        public IActionResult Welcome()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["AuthError"] = "You must be logged in to view this page";
                return RedirectToAction("Index");
            }

            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            
            Console.WriteLine("User Logged In: ", HttpContext.Session.GetInt32("UserId"));
            
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

            // Patient Appointment
            ViewBag.PatientAppt = dbContext.Patients
            .Include(p => p.Appointments)
            .ThenInclude(l => l.Provider)
            .FirstOrDefault(p => p.UserId == userInDb.UserId);

            // List of Appointments Ordered by Date
            // List<Appointment> Appt = dbContext.Appointments
            // .Where(p => p.PatientId == userInDb)
            // .OrderBy()
            // .ToList();
            

            return View();
        }
        
        [HttpGet("test")]
        public IActionResult Test()
        {
            return View();
        }
        [HttpGet("test2")]
        public IActionResult Test2()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }


        // TEST PROVIDER DASHBOARD
        [HttpGet("dashboardtest")]
        public IActionResult ProviderDashboard()
        {
            // int ProviderId = (int) HttpContext.Session.GetInt32("ProviderId");

            // ViewBag.ProviderInfo = dbContext.Providers.FirstOrDefault(p => p.ProviderId == ProviderId);
            return View();                      
        }


    }
}