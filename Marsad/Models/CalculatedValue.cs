using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class CalculatedValue
    {
        [Key]
        public int ID { get; set; }
        public int GeoAreaID { get; set; }
        public GeoArea GeoArea { get; set; }        
        public int EquationYearID { get; set; }
        public EquationYear EquationYear { get; set; }
        public float Value { get; set; }
    }
}