using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


namespace VirtualClinic.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        // One to many relationship with User
        public int UserId { get; set; }
        public User User { get; set; }

        // Patient Information

        // Height and Weight
        public string Height { get; set; }
        public string Weight { get; set; }

        // Blood Type
        public string BloodType { get; set; }

        // Relationship tables
        // Allergies
        public List<Allergy> Allergies { get; set; }

        // Medications
        public List<Medication> Medications { get; set; }

        // Appointments
        public List<Appointment> Appointments { get; set; }

        // Medical History
        public List<MedicalHistory> MedicalHistory { get; set; }
    }
}