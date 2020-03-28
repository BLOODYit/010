using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class ElementYearValue
    {
        public ElementYearValue()
        {

        }
        [Key]
        public int ID { get; set; }
        public int ElementID { get; set; }
        public virtual Element Element { get; set; }
        public double Value { get; set; }
        public int Year { get; set; }
        public string ApplicationUserID { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int GeoAreaID { get; set; }
        public GeoArea GeoArea { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsCommited { get; set; } = false;
    }
}