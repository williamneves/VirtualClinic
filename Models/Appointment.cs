using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace VirtualClinic.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        // Date and Time
        [Required(ErrorMessage = "*Required")]
        [Display(Name = "Date", Prompt = "Date")]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }

        // Appointment Status
        public string Status { get; set; }

        // Relationship with Provider
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }

        // Relationship with Patient
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        
        // Relationship with Medical Notes
        public List<MedicalNote> MedicalNotes { get; set; }
    }
}
