using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class GeoAreaBundle
    {
       
        [Key]
        public int ID { get; set; }
        
        public int GeoAreaID { get; set; }
        public GeoArea GeoArea { get; set; }

        [ForeignKey("GeoArea")]
        public int ChildGeoAreaID { get; set; }
        public virtual GeoArea ChildGeoArea { get; set; }
    }

}