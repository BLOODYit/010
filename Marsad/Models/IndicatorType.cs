using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class IndicatorType
    {

        public IndicatorType()
        {
            this.Indicators = new List<Indicator>();
        }

        [Key]
        public int ID { get; set; }
       
        [Required]
        [MaxLength(255)]        
        public string Name { get; set; }

        public List<Indicator> Indicators { get; set; }
    }
}