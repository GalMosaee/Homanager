using Homanager.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Homanager.Models
{
    public class AppUser : IdentityUser
    { 
        [DisplayName("First name")]
        [PersonalData]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        [PersonalData]
        public string LastName { get; set; }

        //Also username
        [Required]
        [DisplayName("Email")]
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        public string Email { get; set; }
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword

        [DataType(DataType.Date)]
        [DisplayName("Date of birth")]
        [PersonalData]
        public DateTime DateOfBirth { get; set; }

        [DisplayName("Date opened")]
        public DateTime DateOpened { get; set; }

    }
}
