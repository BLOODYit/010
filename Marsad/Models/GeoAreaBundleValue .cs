namespace Marsad.Models
{
    public class GeoAreaBundleValue
    {
        public int ID { get; set; }
        public int GeoAreaBundleID { get; set; }
        public virtual GeoAreaBundle GeoAreaBundle { get; set; }
        public int EquationID { get; set; } 
        public Equation Equation { get; set; }
        public float ElementValue { get; set; }

    }
}