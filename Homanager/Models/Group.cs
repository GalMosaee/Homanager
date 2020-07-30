using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Homanager.Models;
using Microsoft.AspNetCore.Identity;

namespace Homanager.Models
{
    public class Group
    {
        [Key]
        [DisplayName("Group ID")]
        public string GroupId { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }
        
        [DisplayName("Date opened")]
        public DateTime DateOpened { get; set; }
        
        [DisplayName("Owner ID")]
        public string OwnerId { get; set; }
        
        [DisplayName("Owner")]
        public AppUser Owner { get; set; }
        
        [DisplayName("Participants")]
        public ICollection<UserGroup> Participants { get; set; }

        [DisplayName("Carts")]
        public ICollection<Cart> Carts { get; set; }

    }
}
