using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class Equation
    {
        public Equation()
        {
            this.Elements = new List<Element>();
        }

        [Key]
        public int ID { get; set; }

        [Required]
        public int IndicatorID { get; set; }
        public Indicator Indicator { get; set; }

        [Required]
        public int Year { get; set; }

        public string EquationText { get; set; }        

        public List<Element> Elements { get; set; }
    }
}