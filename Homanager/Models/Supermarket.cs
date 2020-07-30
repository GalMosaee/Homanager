using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Homanager.Models
{
    public class Supermarket
    {
        public string SupermarketId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]  
        public double Latitude { get; set; }
    }
}
