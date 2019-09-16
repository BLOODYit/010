using System.ComponentModel.DataAnnotations;

namespace Marsad.Models
{
    public class GeoAreaValue
    {
        public int ID { get; set; }
        [Display(Name="النظاق الجغرافي")]
        public int GeoAreaID { get; set; }
        public virtual GeoArea GeoArea { get; set; }
        [Display(Name ="المعادلة")]
        public int EquationID { get; set; }
        public Equation Equation { get; set; }
        public float ElementValue { get; set; }

    }
}