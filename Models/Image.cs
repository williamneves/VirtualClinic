using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;



namespace VirtualClinic.Models
{
    public class Image:MyContext
    {
        public Image(DbContextOptions<Image> options): base(options)
        {

        }

        public DbSet<User> Images { get; set; }
    }
}