namespace Marsad.Models
{
    public class GeoAreaValue
    {
        public int ID { get; set; }
        public int GeoAreaID { get; set; }
        public virtual GeoArea GeoArea { get; set; }
        public int EquationID { get; set; }
        public Equation Equation { get; set; }
        public float ElementValue { get; set; }

    }
}