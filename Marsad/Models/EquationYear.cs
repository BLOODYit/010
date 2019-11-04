using System.ComponentModel.DataAnnotations;

namespace Marsad.Models
{
    public class EquationYear
    {
        [Key]
        public int ID { get; set; }
        public int EquationID { get; set; }
        public Equation Equation { get; set; }
        [Display(Name = "السنة")]
        public int Year { get; set; }
    }
}