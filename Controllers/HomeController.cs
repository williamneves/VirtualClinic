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
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

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

        [HttpGet("postroomvideo")]
        public IActionResult PostRoomVideo()
        {

            // var url = "https://api.daily.co/v1/rooms/";
            //
            // var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            // httpRequest.Method = "POST";
            //
            // httpRequest.ContentType = "application/json";
            // httpRequest.Headers["Authorization"] = "Bearer 03aca7a0426cbe7eb6b6fb068dba0fd4c9b25aff6413607a606f9cdfd7ea9d61";
            //
            // var data = "{\"properties\":{\"exp\":`expr";
            //
            // using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            // {
            //     streamWriter.Write(data);
            // }
            //
            // var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            // using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            // {
            //     var result = streamReader.ReadToEnd();
            // }
            //
            // Console.WriteLine(httpResponse.StatusCode);

            // var roomName = "apptId2";
            //
            // Console.WriteLine("PostRoomVideo");
            //
            // var url = "https://api.daily.co/v1/rooms/";
            //
            // var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            // httpRequest.Method = "POST";
            //
            // httpRequest.ContentType = "application/json";
            // httpRequest.Headers["Authorization"] = "Bearer 03aca7a0426cbe7eb6b6fb068dba0fd4c9b25aff6413607a606f9cdfd7ea9d61";
            //
            // var data = @"{""name"": ""apptId1"",
            // ""privacy"": ""public"",
            // ""properties"" : {
            // ""start_audio_off"":true,
            // ""start_video_off"":true}}";
            //
            // JObject json = JObject.Parse(data);
            // json["name"] = roomName;
            // data = json.ToString();
            //
            //
            //
            // Console.WriteLine("PostRoomVideo sending data: ");
            // using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            // {
            //     streamWriter.Write(data);
            // }
            //
            // Console.WriteLine("PostRoomVideo receive data: ");
            // var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            // using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            // {
            //     Console.WriteLine("PostRoomVideo receive data: ");
            //     var result = streamReader.ReadToEnd();
            //     Console.WriteLine(result);
            // }
            //
            // Console.WriteLine("\n\n\n\n", httpResponse.StatusCode);

            return View();
        }

        // VideoAPI CreateRoom
        [HttpGet("videoapi/createroom")]
        public IActionResult CreateRoom()
        {
            var url = "https://api.daily.co/v1/rooms/";
            
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            
            httpRequest.ContentType = "application/json";
            httpRequest.Headers["Authorization"] = "Bearer 03aca7a0426cbe7eb6b6fb068dba0fd4c9b25aff6413607a606f9cdfd7ea9d61";
            
            var data = @"{""properties"" : {
                          ""exp"": ""expr"",
                          ""enable_chat"":true,
                          ""eject_at_room_exp"":true,
                          ""start_audio_off"":false,
                          ""start_video_off"":false,
                          ""max_participants"":2}}";
            
            // Get total seconds from a 00:00:00 UTC on 1st January 1970 until now
            var seconds = (DateTime.UtcNow.AddMinutes(90) - new DateTime(1970, 1, 1)).TotalSeconds;

            JObject json = JObject.Parse(data);
            json["properties"]["exp"] = seconds;
            Console.WriteLine(json["properties"]["exp"]);
            data = json.ToString();
            Console.WriteLine($"CreateRoom sending data: {data}");
            
            Console.WriteLine("CreateRoom sending data: ");
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            
            var result = "";
            
            Console.WriteLine("CreateRoom receive data: ");
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
                Console.WriteLine(result);
            }
            
            Console.WriteLine(httpResponse.StatusCode);
            Console.WriteLine(result);
            
            JObject jsonResult = JObject.Parse(result);
            Console.WriteLine(jsonResult["name"]);
            Console.WriteLine(jsonResult["url"]);
            
            
            return Json(new{
                videoRoom = (string)jsonResult["name"],
                videoUrl = (string)jsonResult["url"]
            });
        }
        
        // Save videoCall Url in appointment
        [HttpPost("/provider/appt/setvideourl/{videoRoom}/{apptId}/")]
        public IActionResult SetVideoUrl(string videoRoom, int apptId)
        {
            Console.WriteLine("New Room Object: " + videoRoom);
            Console.WriteLine("New Room Object: " + apptId);
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["AuthError"] = "You must be logged in to view this page";
                return RedirectToAction("Index", "Home");
            }
            
            // Get the Appointment with the apptId
            Console.WriteLine("Get the Appointment with the apptId");
            var apptInDb = dbContext.Appointments
                .FirstOrDefault(p => p.AppointmentId == apptId);
            
            // If has videoUrl in apptInDb, post delete request to delete the room
            if (apptInDb.videoUrl != null && !apptInDb.roomIsExpired())
            {
                var url = $"https://api.daily.co/v1/rooms/{apptInDb.videoRoom}";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "DELETE";
                
                httpRequest.ContentType = "application/json";
                httpRequest.Headers["Authorization"] = "Bearer 03aca7a0426cbe7eb6b6fb068dba0fd4c9b25aff6413607a606f9cdfd7ea9d61";


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }

                Console.WriteLine(httpResponse.StatusCode);
            }

            var dailyDomain = "https://williamneves.daily.co/";
            
            // Update the videoUrl in the Appointment
            Console.WriteLine("Update the videoUrl in the Appointment");
            apptInDb.videoUrl = $"{dailyDomain}{videoRoom}";
            apptInDb.videoRoom = videoRoom;
            apptInDb.videoRoomCreateDate = DateTime.UtcNow;
            dbContext.SaveChanges();
            
            // Return Json result
            return Json("OK");
        }
        
        // Change appointment status
        [HttpPost("/provider/appt/status/{apptId}/{status}/")]
        public IActionResult ChangeStatus(int apptId, string status)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["AuthError"] = "You must be logged in to view this page";
                return RedirectToAction("Index", "Home");
            }
            
            // Get the Appointment with the apptId
            var apptInDb = dbContext.Appointments
                .FirstOrDefault(p => p.AppointmentId == apptId);
            
            // Update the status in the Appointment
            apptInDb.Status = status;
            dbContext.SaveChanges();
            
            // Return Json result
            return Json("OK");
        }

    }
}