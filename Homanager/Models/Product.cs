using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Homanager.Models
{
    public class Product
    {
        public Product(string name)
        {
            Name = name;
        }

        public string ProductId { get; set; }

        [Required]
        [DisplayName("Product")]
        public string Name { get; set; }


        [DisplayName("Category")]
        public ProductCategory Category { get; set; }
    }
}
