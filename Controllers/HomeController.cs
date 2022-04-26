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
                                    .ThenInclude(u => u.User)
                                    .Include(p => p.Appointments)
                                    .ThenInclude(m => m.MedicalNotes)
                                    .FirstOrDefault(p => p.UserId == userInDb.UserId);

            // // List of Appointments Ordered by Date
            // ViewBag.AllAppointments = dbContext.Appointments
            //     .Include(p => p.Patient)
            //     .Include(p => p.Provider)
            //     .Where(p => p.PatientId == userInDb.UserId)
            //     .OrderBy(p => p.DateTime)
            //     .ToList();
            
            // Next appointment (not in the past)
            ViewBag.NextAppointment = dbContext.Appointments
                .Include(p => p.Patient)
                .Include(p => p.Provider)
                .ThenInclude(u => u.User)
                .Where(p => p.PatientId == userInDb.UserId && p.DateTime >= DateTime.Now.AddHours(-1))
                .OrderBy(p => p.DateTime)
                .FirstOrDefault();
            

            return View();
        }
        
        [HttpGet("test")]
        public IActionResult Test()
        {
            return View();
        }
        
        // Json veiw for Medical Notes
        [HttpGet("json/medicalnotes/{id}")]
        public IActionResult MedicalNotes(int id)
        {
            MedicalNote note = dbContext.MedicalNotes.FirstOrDefault(n => n.MedicalNoteId == id);
            return Json(note);
        }
        
        // Jsaon view for Appointments
        [HttpGet("json/appointments/{id}")]
        public IActionResult Appointments(int id)
        {
            Appointment appointment = dbContext.Appointments.FirstOrDefault(a => a.AppointmentId == id);
            return Json(appointment);
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

        // TEST PROVIDERDOCUMENATION

        [HttpGet("providerdocumentation")]
        public IActionResult ProviderDocumentation()
        {
            
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
                                    .ThenInclude(u => u.User)
                                    .Include(p => p.Appointments)
                                    .ThenInclude(m => m.MedicalNotes)
                                    .FirstOrDefault(p => p.UserId == userInDb.UserId);

            // // List of Appointments Ordered by Date
            // ViewBag.AllAppointments = dbContext.Appointments
            //     .Include(p => p.Patient)
            //     .Include(p => p.Provider)
            //     .Where(p => p.PatientId == userInDb.UserId)
            //     .OrderBy(p => p.DateTime)
            //     .ToList();
            
            // Next appointment (not in the past)
            ViewBag.NextAppointment = dbContext.Appointments
                .Include(p => p.Patient)
                .Include(p => p.Provider)
                .ThenInclude(u => u.User)
                .Where(p => p.PatientId == userInDb.UserId && p.DateTime >= DateTime.Now.AddHours(-1))
                .OrderBy(p => p.DateTime)
                .FirstOrDefault();
            
            return View();
        }
        
    }
}