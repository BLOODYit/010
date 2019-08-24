using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class EquationElement
    {
        public EquationElement()
        {
        }

        [Key]
        public int ID { get; set; }

        public int EquationID { get; set; }
        public virtual Equation Equation { get; set; }
        public int ElementID { get; set; }
        public virtual Element Element { get; set; }
    }
}