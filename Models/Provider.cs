using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace VirtualClinic.Models
{
    public class Provider
    {
        [Key]
        public int ProviderId { get; set; }

        // One to many relationship with User
        public int UserId { get; set; }
        public User User { get; set; }

        // Provider Information
        [Required(ErrorMessage = "*Required")]
        [MinLength(2, ErrorMessage = "*Needs to be at least 2 characters")]
        [Display(Name = "Specialty", Prompt = "Specialty")]
        public string Specialty { get; set; }

        [Required(ErrorMessage = "*Required")]
        [MinLength(2, ErrorMessage = "*Needs to be at least 2 characters")]
        [Display(Name = "NPI number", Prompt = "NPI number")]
        public string NPInumber { get; set; }

        [Required(ErrorMessage = "*Required")]
        [MinLength(2, ErrorMessage = "*Needs to be at least 2 characters")]
        [Display(Name = "DEA number", Prompt = "DEA number")]
        public string DEAnumber { get; set; }

        public string ExtraField1 { get; set; }
        public string ExtraField2 { get; set; }

        // Provider Bank Information
        [Required(ErrorMessage = "*Required")]
        [MinLength(2, ErrorMessage = "*Needs to be at least 2 characters")]
        [Display(Name = "Bank Name", Prompt = "Bank Name")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "*Required")]
        [MinLength(2, ErrorMessage = "*Needs to be at least 2 characters")]
        [Display(Name = "Account Number", Prompt = "Account Number")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "*Required")]
        [MinLength(2, ErrorMessage = "*Needs to be at least 2 characters")]
        [Display(Name = "Routing Number", Prompt = "Routing Number")]
        public string RoutingNumber { get; set; }

        [MinLength(2, ErrorMessage = "*Needs to be at least 2 characters")]
        [Display(Name = "Additional Information", Prompt = "Additional Information")]
        public string AdditionalInformation { get; set; }

    }
}