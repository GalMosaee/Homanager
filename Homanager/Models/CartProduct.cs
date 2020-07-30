using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Homanager.Models
{
    public class CartProduct
    {
        [Key]
        public string ProductId { get; set; }
        
        [DisplayName("Product")]
        public string ProductName { get; set; }

        public string CartId { get; set; }

        //TODO: for more complex implementation
        //public virtual Product Product { get; set; }
        
        public virtual Cart Cart { get; set; }
        
        public int Quantity { get; set; }

        public string Comment { get; set; }
    }
}
