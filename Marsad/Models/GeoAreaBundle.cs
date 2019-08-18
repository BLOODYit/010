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
        public GeoAreaBundle()
        {
            this.GeoAreas = new HashSet<GeoArea>();
        }
        [Key]
        public int ID { get; set; }

        [Display(Name = "رمز")]
        [Required(ErrorMessage = "برجاء إدخال الكود")]
        public string Code { get; set; }

        [Display(Name = "إسم")]
        [Required(ErrorMessage = "برجاء إدخال الإسم")]
        public string Name { get; set; }

        public virtual ICollection<GeoArea> GeoAreas { get; set; }
    }

}