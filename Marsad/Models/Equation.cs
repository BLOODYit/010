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
            this.EquationElements = new List<EquationElement>();
        }

        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name ="المؤشر")]
        public int IndicatorID { get; set; }
        public Indicator Indicator { get; set; }

        [Required]
        [Display(Name ="السنة")]
        public int Year { get; set; }

        [Display(Name = "المعادلة")]
        public string EquationText { get; set; }        

        public List<EquationElement> EquationElements { get; set; }
    }
}