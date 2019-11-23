using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Marsad.Models
{
    public class EquationYear
    {
        public EquationYear()
        {
            ElementValues = new HashSet<ElementValue>();
        }
        [Key]
        public int ID { get; set; }
        public int EquationID { get; set; }
        public Equation Equation { get; set; }
        [Display(Name = "السنة")]
        public int Year { get; set; }
        public virtual ICollection<ElementValue> ElementValues { get; set; }
    }
}