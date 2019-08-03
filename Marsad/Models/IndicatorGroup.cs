using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class IndicatorGroup
    {
        public IndicatorGroup()
        {
            this.Indicators = new List<Indicator>();
        }
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage ="برجاء إدخال رمز المجموعة")]
        [MaxLength(255)]
        [Display(Name ="رمز المجموعة")]
        public string Code { get; set; }
        
        [MaxLength(255)]
        [Required(ErrorMessage = "برجاء إدخال المجموعة")]
        [Display(Name = "المجموعة")]
        public string Name { get; set; }

        [Display(Name = "الوصف")]
        public string Description { get; set; }

        public List<Indicator> Indicators { get; set; }
    }
}