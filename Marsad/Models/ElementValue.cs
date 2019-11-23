using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class ElementValue
    {
        [Key]
        public int ID { get; set; }
        public int EquationElementID { get; set; }
        public virtual EquationElement EquationElement { get; set; }
        public int EquationYearID { get; set; }
        public virtual EquationYear EquationYear { get; set; }        
        public float Value { get; set; }
        public int? GeoAreaID { get; internal set; }
        public GeoArea GeoArea { get; set; }
        public int? GeoAreaBundleID { get; internal set; }
        public GeoAreaBundle GeoAreaBundle { get; set; }
        public string ApplicationUserID { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CommitedAt { get; set; }
        public bool IsCommited { get; set; }        
    }
}