using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class ElementValue
    {
        [Key]
        public int ID { get; set; }
        public int EquationID { get; set; }
        public Equation Equation { get; set; }
        public int ElementID { get; set; }
        public Element Element { get; set; }
        public int Year { get; set; }
        public float Value { get; set; }
        public int GeoAreaID { get; internal set; }
    }
}