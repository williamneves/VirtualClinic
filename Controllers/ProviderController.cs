using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VirtualClinic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace VirtualClinic.Controllers
{
    public class ProviderController : Controller
    {
        private MyContext dbContext;

        public ProviderController(MyContext context)
        {
            dbContext = context;
        }


        // PROVIDER DASHBOARD
        [HttpGet("providerdashboard")]
        public IActionResult ProviderDashboard()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["AuthError"] = "You must be logged in to view this page";
                return RedirectToAction("Index", "Home");
            }

            int UserId = (int) HttpContext.Session.GetInt32("UserId");

            ViewBag.UserLoggedIn = dbContext.Users.FirstOrDefault(p => p.UserId == UserId);

            return View();
        }

        // Appointment Attendance Provider View

        [HttpGet("provider/apptattendance/{apptId}")]
        public IActionResult ProviderAttendance(int apptId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["AuthError"] = "You must be logged in to view this page";
                return RedirectToAction("Index", "Home");
            }

            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

            ViewBag.UserLoggedIn = userInDb;

            if (userInDb.userType != "provider")
            {
                TempData["AuthError"] = "You must be logged Provider to view this page";
                return RedirectToAction("Index", "Home");
            }

            // Appointment Info
            var apptInfo = dbContext.Appointments
                .Include(p => p.Patient)
                .ThenInclude(p => p.User)
                .Include(p => p.Provider)
                .ThenInclude(u => u.User)
                .Include(p => p.MedicalNotes)
                .FirstOrDefault(p => p.AppointmentId == apptId);

            // Patient Medications
            ViewBag.PatientInfo = dbContext.Patients
                .Include(p => p.Medications)
                .FirstOrDefault(p => p.UserId == apptInfo.Patient.UserId);

            // Patient Allergies
            ViewBag.PatientAllergies = dbContext.Patients
                .Include(p => p.Allergies)
                .FirstOrDefault(p => p.UserId == apptInfo.Patient.UserId);

            // Patient MedHx
            ViewBag.PatientMedHx = dbContext.Patients
                .Include(p => p.MedicalHistory)
                .FirstOrDefault(p => p.UserId == apptInfo.Patient.UserId);

            // Patient Appointment
            ViewBag.PatientAppt = dbContext.Patients
                .Include(p => p.Appointments)
                .ThenInclude(l => l.Provider)
                .ThenInclude(u => u.User)
                .Include(p => p.Appointments)
                .ThenInclude(m => m.MedicalNotes)
                .FirstOrDefault(p => p.UserId == apptInfo.Patient.UserId);

            // Next appointment (not in the past)
            ViewBag.NextAppointment = dbContext.Appointments
                .Include(p => p.Patient)
                .Include(p => p.Provider)
                .ThenInclude(u => u.User)
                .Where(p => p.PatientId == userInDb.UserId && p.DateTime >= DateTime.Now.AddHours(-1))
                .OrderBy(p => p.DateTime)
                .FirstOrDefault();



            ViewBag.ApptInfo = apptInfo;

            ViewBag.MedicalNote = dbContext.MedicalNotes
                .FirstOrDefault(m => m.PatientId == apptInfo.PatientId
                                     && m.ProviderId == apptInfo.ProviderId
                                     && m.AppointmentId == apptInfo.AppointmentId);


            return View();
        }

        // Post Medical Notes Model
        [HttpPost("add-medical-notes-autosave")]
        public IActionResult AddMedicalNotesAutoSave(MedicalNote newNote)
        {
            // Console.WriteLine("\n###############\n");
            // Console.WriteLine("New Medical Note: ");
            // Console.WriteLine(newNote.PatientId);
            // Console.WriteLine(newNote.AP);
            // // Console.WriteLine(;
            // // Console.WriteLine(HPI);
            // Console.WriteLine("\n###############\n");
            //     // dbContext.MedicalNotes.Add(newMedicalNote);
            //     // dbContext.SaveChanges();

            var newNoteInDb = dbContext.MedicalNotes
                .FirstOrDefault(m => m.PatientId == newNote.PatientId
                                     && m.ProviderId == newNote.ProviderId
                                     && m.AppointmentId == newNote.AppointmentId);
            if (newNoteInDb != null)
            {
                newNoteInDb.HPI = newNote.HPI;
                newNoteInDb.PE = newNote.PE;
                newNoteInDb.Summary = newNote.Summary;
                newNoteInDb.AP = newNote.AP;

                // save changes
                dbContext.SaveChanges();

                var response = "Auto-updated Medical Notes";

                return Json(response);
            }
            else
            {
                dbContext.MedicalNotes.Add(newNote);
                dbContext.SaveChanges();

                var response = "Added new Medical Notes";

                return Json(response);
            }

        }

        // Edit Provider Profile
        [HttpGet("edit/providerprofile")]
        public IActionResult EditProviderProfile()
        {
            // User Logged In
            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

            ViewBag.UserLoggedIn = userInDb;

            // Patient Medications
            ViewBag.ProviderInfo = dbContext.Providers
                .Include(p => p.User)
                .FirstOrDefault(p => p.UserId == userInDb.UserId);


            return View();
        }

        // Edit Provider Profile Information
        [HttpPost("edit/providerinfo/")]
        public IActionResult EditPatientInfo(ProviderUpdate UpdatedUser)
        {


            User userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

            // Get the Provider attached to the User
            Provider oldProviderInDb = dbContext.Providers
                .Include(p => p.User)
                .FirstOrDefault(p => p.UserId == userInDb.UserId);

            oldProviderInDb.User.PreferredName = UpdatedUser.PreferredName;
            oldProviderInDb.Specialty = UpdatedUser.Specialty;
            oldProviderInDb.NPInumber = UpdatedUser.NPInumber;
            oldProviderInDb.DEAnumber = UpdatedUser.DEAnumber;
            oldProviderInDb.BankName = UpdatedUser.BankName;
            oldProviderInDb.RoutingNumber = UpdatedUser.RoutingNumber;
            oldProviderInDb.AdditionalInformation = UpdatedUser.AdditionalInformation;

            dbContext.SaveChanges();

            return RedirectToAction("ProviderDashboard");
        }

        // Provider Create Appointment
        // GET METHOD
        [HttpGet("provider/createappointment")]
        public IActionResult ProviderCreateAppt()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["AuthError"] = "You must be logged in to view this page";
                return RedirectToAction("Index", "Home");
            }

            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

            Console.WriteLine("User Logged In: ", HttpContext.Session.GetInt32("UserId"));

            ViewBag.UserLoggedIn = userInDb;

            // Patient Medications

            var providerInfo = dbContext.Providers
                .Include(p => p.User)
                .FirstOrDefault(p => p.UserId == userInDb.UserId);

            ViewBag.ProviderInfo = providerInfo;

            // Get all appointments for the logged in provider with all patients
            ViewBag.ProviderAppointments = dbContext.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Provider)
                .Where(a => a.ProviderId == providerInfo.ProviderId)
                .OrderBy(a => a.DateTime)
                .ToList();

            return View();
        }

        // Provider Create Appointment
        // POST METHOD
        [HttpPost("provider/createappointmentpost")]
        public IActionResult ProviderCreateApptPost(Appointment newAppt)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["AuthError"] = "You must be logged in to view this page";
                return RedirectToAction("Index", "Home");
            }

            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            ViewBag.UserLoggedIn = userInDb;

            // ProviderInfo
            var providerInfo = dbContext.Providers
                .Include(p => p.User)
                .FirstOrDefault(p => p.UserId == userInDb.UserId);

            ViewBag.ProviderInfo = providerInfo;

            // Get all appointments for the logged in provider with all patients
            ViewBag.ProviderAppointments = dbContext.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Provider)
                .Where(a => a.ProviderId == providerInfo.ProviderId)
                .ToList();
            // Validation
            if (ModelState.IsValid)
            {
                // Add Appointment to DB
                dbContext.Appointments.Add(newAppt);
                dbContext.SaveChanges();

                // var response = "Added new Appointment";

                return RedirectToAction("ProviderCreateAppt");
            }

            return RedirectToAction("ProviderCreateAppt");
        }

        //Provider Delete Appointment
        [HttpGet("provider/deleteappointment/{appointmentId}")]
        public IActionResult ProviderDeleteAppt(int appointmentId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["AuthError"] = "You must be logged in to view this page";
                return RedirectToAction("Index", "Home");
            }

            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            ViewBag.UserLoggedIn = userInDb;

            // ProviderInfo
            ViewBag.ProviderInfo = dbContext.Providers
                .Include(p => p.User)
                .FirstOrDefault(p => p.UserId == userInDb.UserId);
            // Validation
            if (userInDb.userType == "provider")
            {
                var apptToDelete = dbContext.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
                dbContext.Appointments.Remove(apptToDelete);
                dbContext.SaveChanges();
                return RedirectToAction("ProviderCreateAppt", "Provider");
            }

            TempData["AuthError"] = "You must be a Provider to view this page";
            return RedirectToAction("ProviderCreateAppt", "Provider");
        }

        // Get all Patients
        [HttpGet("allpatients")]
        public IActionResult AllPatients()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["AuthError"] = "You must be logged in to view this page";
                return RedirectToAction("Index", "Home");
            }

            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

            // Catch the provider in Db
            var providerInDb = dbContext.Providers
                .Include(p => p.User)
                .FirstOrDefault(p => p.UserId == userInDb.UserId);

            // ViewBag.AllPatients = dbContext.

            // All the appoitments for the ProviderId
            var AllAppointments = dbContext.Appointments
                .Include(p => p.Patient)
                .Include(p => p.Provider)
                .Where(p => p.ProviderId == providerInDb.ProviderId)
                .OrderBy(p => p.DateTime)
                .ToList();

            // All users with have an appment with the provider
            var AllPatients = dbContext.Patients
                .Include(p => p.User)
                .Include(p => p.Appointments)
                .Where(p => p.Appointments.Any(a => a.ProviderId == providerInDb.ProviderId))
                .ToList();

            // Create the veiewbags
            ViewBag.AllAppointments = AllAppointments;
            ViewBag.AllPatients = AllPatients;
            ViewBag.ProviderInfo = providerInDb;
            ViewBag.UserLoggedIn = userInDb;

            return View();
        }

        // Single Patient Provider View
        [HttpGet("/patientinfo/{patientId}")]
        public IActionResult SinglePtProviderView(int patientId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["AuthError"] = "You must be logged in to view this page";
                return RedirectToAction("Index", "Home");
            }

            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

            // Catch the provider in Db
            var providerInDb = dbContext.Providers
                .Include(p => p.User)
                .FirstOrDefault(p => p.UserId == userInDb.UserId);

            // ViewBag.AllPatients = dbContext.

            // All the appointments for that the patient has
            var AllAppointments = dbContext.Appointments
                .Include(p => p.Patient)
                .Include(p => p.Provider)
                .Where(p => p.PatientId == patientId)
                .OrderBy(p => p.DateTime)
                .ToList();

            // Create the veiewbags
            ViewBag.AllAppointments = AllAppointments;
            ViewBag.ProviderInfo = providerInDb;
            ViewBag.UserLoggedIn = userInDb;
            // Patient Info

            ViewBag.PatientInfo = dbContext.Patients
                .Include(p => p.Medications)
                .Include(p => p.ReportedMedications)
                .SingleOrDefault(p => p.PatientId == patientId);

            // Patient Allergies
            ViewBag.PatientAllergies = dbContext.Patients
                .Include(p => p.Allergies)
                .SingleOrDefault(p => p.PatientId == patientId);

            // Patient MedHx
            ViewBag.PatientMedHx = dbContext.Patients
                .Include(p => p.MedicalHistory)
                .SingleOrDefault(p => p.PatientId == patientId);

            // Patient Appointment
            ViewBag.PatientAppt = dbContext.Patients
                .Include(p => p.Appointments)
                .ThenInclude(l => l.Provider)
                .ThenInclude(u => u.User)
                .Include(p => p.Appointments)
                .ThenInclude(m => m.MedicalNotes)
                .SingleOrDefault(p => p.PatientId == patientId);

            // Next appointment (not in the past)
            ViewBag.NextAppointment = dbContext.Appointments
                .Include(p => p.Patient)
                .Include(p => p.Provider)
                .ThenInclude(u => u.User)
                .Where(p => p.PatientId == patientId && p.DateTime >= DateTime.Now.AddHours(-1))
                .OrderBy(p => p.DateTime)
                .FirstOrDefault();

            return View();
        }


=======
// Provider Messages page
        [HttpGet("/providerinbox")]
        public IActionResult ProviderInbox()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["AuthError"] = "You must be logged in to view this page";
                return RedirectToAction("Index", "Home");
            }

            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

            ViewBag.AllMessages = dbContext.Messages
                                .Include(p => p.Patient)
                                .Include(p => p.Provider)
                                .Where(p => p.PatientId == userInDb.UserId)
                                .ToList();
            
            ViewBag.AllPatients = dbContext.Patients
                                .Include(p => p.User)
                                .ToList();

            ViewBag.UserLoggedIn = userInDb;

            ViewBag.Provider = dbContext.Providers
                            .FirstOrDefault(p => p.User.UserId == userInDb.UserId);
            
            return View();
        }
        
        // Messages
        [HttpGet("/providerinbox/partial/{providerId}/{patientId}")]
        public IActionResult ProviderInboxParital(int providerId, int patientId)
        {
            ViewBag.ProviderId = providerId;
            ViewBag.PatientId = patientId;

            ViewBag.Messages = dbContext.Messages
                                .Include(p => p.Patient)
                                .Include(p => p.Provider)
                                .Where(p => p.PatientId == patientId && p.ProviderId == providerId)
                                .ToList();
            
            ViewBag.Patients = dbContext.Patients.FirstOrDefault(p => p.PatientId == patientId);

            return PartialView(@"~/Views/Shared/_InboxProvider.cshtml");
        }

        // Messages
        [HttpGet("/updateproviderinbox/partial/{text}/{providerId}/{patientId}")]
        public IActionResult ProviderInbox(Message newMessage, int providerId, int patientId)
        {

            dbContext.Add(newMessage);
            dbContext.SaveChanges();

            ViewBag.ProviderId = providerId;
            ViewBag.PatientId = patientId;
            
            ViewBag.Messages = dbContext.Messages
                                .Include(p => p.Patient)
                                .Include(p => p.Provider)
                                .Where(p => p.PatientId == patientId && p.ProviderId == providerId)
                                .ToList();
            
            ViewBag.Patients = dbContext.Patients.FirstOrDefault(p => p.PatientId == patientId);


            return PartialView(@"~/Views/Shared/_InboxProvider.cshtml");
        }
        
        // MESSAGE
    //     [HttpPost("sendptmessage")]
    //     public IActionResult SendMessage(Message newMessage)
    //     {
    //         dbContext.Add(newMessage);
    //         dbContext.SaveChanges();
    //         return RedirectToAction("ProviderInbox");
    //     }
    }
}