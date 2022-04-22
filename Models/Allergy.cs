using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;



namespace VirtualClinic.Models
{
    public class Allergy
    {
        [Key] public int AllergyId { get; set; }

        public string Name { get; set; }
        public string Reaction { get; set; }
        
        // Foreign Key and Navigation Property
        public int PatientId { get; set; }
        public Patient Client { get; set; }

        // Created and Updated Date
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        
        // Methods

    }
}