using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;



namespace VirtualClinic.Models
{
    public class Message
    {
        [Key] 
        public int MessageId { get; set; }
        public string Text { get; set; }
        
        // Foreign Key and Navigation Property
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int ProviderId { get; set; }
        public Provider Provider { get; set; }
        // public int ThreadId { get; set; }
        // public Thread Thread { get; set; } 

        public string UploadURL { get; set; }


        // Created and Updated Date
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        // Methods

    }
}