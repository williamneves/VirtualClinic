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
    public class PatientController : Controller
    {
        private MyContext dbContext;

        public PatientController( MyContext context)
        {
            dbContext = context;
        }

        // patient dashboard
        [HttpGet("patientdashboard")]
        public IActionResult PatientDashboard()
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
            ViewBag.PatientInfo = dbContext.Patients
                                    .Include(p => p.Medications)
                                    .Include(p => p.ReportedMedications)
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

            // Console.WriteLine(ViewBag.PatientAppt.Appointments.MedicalNotes.Count);
            // Console.WriteLine(ViewBag.PatientAppt.MedicalNotes);
            
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

        // Json view for Medical Notes
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

        // Edit Patient Profile Information
        [HttpPost("edit/patientinfo/")]
        public IActionResult EditPatientInfo(User UpdatedUser)
        {
            User OldPatient = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

            OldPatient.PreferredName = UpdatedUser.PreferredName;
            OldPatient.Pronouns = UpdatedUser.Pronouns;
            OldPatient.Email = OldPatient.Email;
            OldPatient.Password = OldPatient.Password;
             // Initialize hasher object
            // PasswordHasher<User> Hasher = new PasswordHasher<User>();
            // // Hash password
            // OldPatient.Password = Hasher.HashPassword(UpdatedUser, UpdatedUser.Password);
            OldPatient.StreetAddress = UpdatedUser.StreetAddress;
            OldPatient.City = UpdatedUser.City;
            OldPatient.State = UpdatedUser.State;
            OldPatient.Zipcode = UpdatedUser.Zipcode;
            OldPatient.PhoneNumber = UpdatedUser.PhoneNumber;
            
            dbContext.SaveChanges();

            return RedirectToAction("PatientDashboard");
        }
    
        // Get the Edit Page for Medical Info
        [HttpGet("edit/medicalinfo")]
        public IActionResult UpdateMedicalInfo()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["AuthError"] = "You must be logged in to view this page";
                return RedirectToAction("Index", "Home");
            }

            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            
            ViewBag.UserLoggedIn = userInDb;

            // Patient Medications
            ViewBag.PatientInfo = dbContext.Patients
                                    .Include(p => p.Medications)
                                    .Include(l => l.ReportedMedications)
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

        // Add medications (provider in appt)
        [HttpPost("addmed")]

        public IActionResult AddMed(Medication NewMed)
        {
            NewMed.PatientId = (int) HttpContext.Session.GetInt32("UserId");

            dbContext.Add(NewMed);
            dbContext.SaveChanges();
            return RedirectToAction("UpdateMedicalInfo");

        }
        // Add medications (pt reported)
        [HttpPost("addptmed")]

        public IActionResult AddPtMed(ReportedMedication NewMed)
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

        // Remove Medication (prescribed)
        [HttpGet("remove/medication/{id}")]
        public IActionResult RemoveMed(int id)
        {
            Medication MedToRemove = dbContext.Medications.FirstOrDefault(d => d.MedicationId == id);
            dbContext.Medications.Remove(MedToRemove);
            dbContext.SaveChanges();
            return RedirectToAction("UpdateMedicalInfo");
        }

        // Remove Medication (patient reported)
        [HttpGet("remove/ptmedication/{id}")]
        public IActionResult RemovePtMed(int id)
        {
            ReportedMedication MedToRemove = dbContext.ReportedMedications.FirstOrDefault(d => d.ReportedMedicationId == id);
            dbContext.ReportedMedications.Remove(MedToRemove);
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
        
        
        // Patient Inbox
        [HttpGet("/patientinbox")]
        public IActionResult PatientInbox()
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
            
            ViewBag.AllProviders = dbContext.Providers
                                .Include(p => p.User)
                                .ToList();

            ViewBag.UserLoggedIn = userInDb;

            ViewBag.Patient = dbContext.Patients
                            .FirstOrDefault(p => p.User.UserId == userInDb.UserId);

            return View();
        }

        // Partial Message for Patient
        [HttpGet("/patientinbox/partial/{writerId}/{providerId}/{patientId}")]
        public IActionResult PatientInboxPartial(int writerId, int providerId, int patientId)
        {
            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            ViewBag.UserLoggedIn = userInDb;
            ViewBag.UserId = userInDb.UserId;
            
            var result = dbContext.Messages.Where(p => p.PatientId == patientId && p.ProviderId == providerId).ToList();
                if (result != null)
                {
                    foreach(Message i in result)
                    {
                        i.Read = true;
                    }
                    dbContext.SaveChanges();
                }

            ViewBag.ProviderId = providerId;
            ViewBag.PatientId = patientId;

            ViewBag.Messages = dbContext.Messages
                                .Include(p => p.Patient)
                                .Include(p => p.Provider)
                                .Where(p => p.PatientId == patientId && p.ProviderId == providerId)
                                .ToList();
            
            ViewBag.Providers = dbContext.Providers.FirstOrDefault(p => p.ProviderId == providerId);

            ViewBag.ReadMessages = dbContext.Messages
                                .Where(p => p.PatientId == patientId && p.ProviderId == providerId)
                                .Any(p => p.Read == false);

            Console.WriteLine(ViewBag.ReadMessages);

            return PartialView(@"~/Views/Shared/_InboxPt.cshtml");
        }
        
        // Post Message
        [HttpGet("/updateinbox/partial/{text}/{writerId}/{providerId}/{patientId}")]
        public IActionResult PatientInbox(Message newMessage, int writerId, int providerId, int patientId)
        {
            var userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

            ViewBag.UserLoggedIn = userInDb;
        
            dbContext.Add(newMessage);
            dbContext.SaveChanges();

            ViewBag.ProviderId = providerId;
            ViewBag.UserId = userInDb.UserId;
            ViewBag.PatientId = patientId;

            ViewBag.Messages = dbContext.Messages
                                .Include(p => p.Patient)
                                .Include(p => p.Provider)
                                .Where(p => p.PatientId == patientId && p.ProviderId == providerId)
                                .ToList();
            
            ViewBag.Providers = dbContext.Providers.FirstOrDefault(p => p.ProviderId == providerId);

                        ViewBag.ReadMessages = dbContext.Messages
                                .Where(p => p.PatientId == patientId && p.ProviderId == providerId)
                                .Any(p => p.Read == false);

            Console.WriteLine(ViewBag.ReadMessages);

            return PartialView(@"~/Views/Shared/_InboxPt.cshtml");
        }

        // Post Message
        // [HttpPost("senddrmessage")]
        // public IActionResult SendMessage(Message newMessage)
        // {
        //     dbContext.Add(newMessage);
        //     dbContext.SaveChanges();
        //     return RedirectToAction("PatientInbox");
        // }
    }
}