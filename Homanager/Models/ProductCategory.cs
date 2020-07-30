using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Homanager.Models
{
    public class ProductCategory
    {
        public string ProductCategoryId { get; set;  }

        [Required]
        public string Name { get; set; }
    }
}
