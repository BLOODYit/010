using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class IndicatorGroup
    {

        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Code { get; set; }

        [Required]
        [MaxLength(255)]        
        public string Name { get; set; }

        public string Description { get; set; }
    }
}