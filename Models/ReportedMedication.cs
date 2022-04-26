using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;



namespace VirtualClinic.Models
{
    public class ReportedMedication
    {
        [Key] public int ReportedMedicationId { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "*Required")]
        public string Description { get; set; }

        // Patient Foreign Key and Navigation Property
        public int PatientId { get; set; }
        public Patient Client { get; set; }

        // Created and Updated Date
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        
        // Methods

    }
}