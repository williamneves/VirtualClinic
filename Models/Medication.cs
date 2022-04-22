using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;



namespace VirtualClinic.Models
{
    public class Medication
    {
        [Key] public int MedicationId { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "*Required")]
        public string Description { get; set; }
        
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