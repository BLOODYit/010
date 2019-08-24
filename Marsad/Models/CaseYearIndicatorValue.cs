namespace Marsad.Models
{
    public class CaseYearIndicatorValue
    {
        public int ID { get; set; }
        public int CaseYearIndicatorID { get; set; }
        public virtual CaseYearIndicator CaseYearIndicator { get; set; }

        public int ElementID { get; set; }
        public virtual Element Element { get; set; }

        public float ElementValue { get; set; }
    }
}