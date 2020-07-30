using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Homanager.Models
{
    public class Cart
    {
        [Key]
        [DisplayName("Cart ID")]
        public string CartId { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }
        
        [DisplayName("Date opened")]
        public DateTime DateOpened { get; set; }
        
        [DisplayName("Group Id")]
        public string OwnerGroupId { get; set; }

        [DisplayName("Group")]
        public virtual Group OwnerGroup { get; set; }

        [DisplayName("Cart products")]
        public virtual ICollection<CartProduct> CartProducts { get; set; }
    }
}
