using Homanager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Homanager.Models
{
    public class UserGroup
    {
        [Column(Order=1)]
        public string UserId { get; set; }
        
        [Column(Order = 2)]
        public string GroupId { get; set; }

        public virtual AppUser User { get; set; }

        public virtual Group Group { get; set; }
    }
}
