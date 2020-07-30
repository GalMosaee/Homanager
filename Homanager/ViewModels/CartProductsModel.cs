using Homanager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Homanager.ViewModels
{
    public class CartProductsModel
    {
        public string CartId { get; set; }
        public Cart Cart { get; set; }
        
        [DisplayName("Product")]
        public string ProductName { get; set; }
        
        public int Quantity { get; set; }
        
        public string Comment { get; set; }

        public CartProduct AddProduct { get; set; }
        public IEnumerable<CartProduct> Products { get; set; }
    }
}
