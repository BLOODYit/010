using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        [Display(Name ="رمز المجموعة")]
        [Remote("IsExist", "IndicatorGroups", AdditionalFields = "ID", ErrorMessage = "رمز المجموعة يجب الا يتكرر")]
        [Range(1, int.MaxValue, ErrorMessage = "يجب ان يكون رمز المجموعة رقم موجب")]
        public int Code { get; set; }
        
        [MaxLength(255)]
        [Required(ErrorMessage = "برجاء إدخال المجموعة")]
        [Display(Name = "المجموعة")]
        public string Name { get; set; }

        [Display(Name = "الوصف")]
        public string Description { get; set; }

        public List<Indicator> Indicators { get; set; }
    }
}