using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;


namespace VirtualClinic.Models
{
    public class User
    {
        [Key] public int UserId { get; set; }

        // Hidden field for all users
        [Required]
        public string userType { get; set; } = "patient"; 

        [Required(ErrorMessage = "*Required")]
        [MinLength(2, ErrorMessage = "*Needs to be at least 2 characters")]
        [Display(Name = "First Name", Prompt = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "*Required")]
        [MinLength(2, ErrorMessage = "*Needs to be at least 2 characters")]
        [Display(Name = "Last Name", Prompt = "Last Name")]
        public string LastName { get; set; }

        // Optional MiddleName
        [Display(Name = "Middle Name", Prompt = "Middle Name")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "*Required")]
        [EmailAddress]
        [Display(Name = "Email", Prompt = "Email")]
        public string Email { get; set; }

        // Date of birth
        [Required(ErrorMessage = "*Required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth", Prompt = "Date of Birth")]
        public DateTime DOB { get; set; }

        // User Sex type
        [Required(ErrorMessage = "*Required")]
        [Display(Name = "Sex", Prompt = "Select your Sex")]
        public string Sex { get; set;}
        
        // How the user like to be called
        [Display(Name = "Pronouns", Prompt = "If you want, select above")]
        public string Pronouns { get; set;}
        
        [Display(Name = "Preferred Name", Prompt = "Preferred Name")]
        public string PreferredName { get; set; }

        // Image profile
        public string ImageProfile { get; set; }
        [NotMapped]
        [Display(Name = "Upload File")]
        public IFormFile ImageFile { get; set; }

        // User Address
        public string StreetAddress { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }

        // User Phone Number
        [Phone]
        [Display(Name = "Phone Number (xxx-xxx-xxxx)", Prompt = "Phone Number (xxx-xxx-xxxx)")]
        public string PhoneNumber { get; set; }


        // User Password
        [Required(ErrorMessage = "*Required")]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", Prompt = "Password")]

        public string Password { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "*Required")]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password", Prompt = "Confirm Password")]
        public string Confirm { get; set; }
        
        [NotMapped]
        [Display(Name="Provider Code", Prompt = "Provider Code")]
        // Regex to match ^ABXY$
        [RegularExpression(@"^ABXY$", ErrorMessage = "*Invalid Provider Code")]
        [MinLength(4, ErrorMessage = "*Needs to be at least 4 characters")]
        public string AuthCode { get; set; }


        // Created and Updated Date
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        








        // Methods
        // Get Full Name
        public string getFullName()
        {
            return $"{FirstName} {LastName}";
        }

        // public int get_age()  
        //     {  
        //     int age = 0;  
        //     age = DateTime.Now.AddYears(-this..Year).Year;  
        //     return age;
        // }  
        public int getAgeInYear()
        {
            var today = DateTime.Today;
            var age = today.Year - this.DOB.Year;
            if (this.DOB.Date > today.AddYears(-age)) age--;
            return (int) age;
        }

    }
}