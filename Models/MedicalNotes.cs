using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;



namespace VirtualClinic.Models
{
    public class MedicalNote
    {
        [Key] public int MedicalNoteId { get; set; }

        // History of Present Illness
        [Required(ErrorMessage = "*Required")]
        public string HPI { get; set; }

        // Physcial Exam
        [Required(ErrorMessage = "*Required")]
        public string PE { get; set; }
        // Assessment and Plan

        [Required(ErrorMessage = "*Required")]
        public string AP { get; set; }
        
        // Patient Foreign Key and Navigation Property
        public int PatientId { get; set; }
        public Patient Client { get; set; }

        // Provider Foreign Key and Navigation Property
        public int ProviderId { get; set; }
        public Provider Clinician { get; set; }

        
        // Appointment Foreign Key and Navigation Property

        public int  AppointmentId { get; set; }
        public Appointment Visit { get; set; }


        // Created and Updated Date
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        
        // Methods

    }
}