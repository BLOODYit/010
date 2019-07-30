using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class Bundle
    {
        public Bundle()
        {
            this.Indicators = new List<Indicator>();
        }

        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage ="برجاء إدخال كود الحزمة")]
        [MaxLength(255,ErrorMessage ="كود الحزمة يجب الا يتعدى 255 حرف")]       
        [Display(Name="كود الحزمة")]
        public string Code { get; set; }

        [Required(ErrorMessage = "برجاء إدخال إسم الحزمة")]
        [MaxLength(255,ErrorMessage ="إسم الحزمة يجب الا يتعدى 255 حرف")]
        [Display(Name="إسم الحزمة",ShortName ="الحزمة")]
        public string Name { get; set; }

        [Display(Name = "وصف الحزمة")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public List<Indicator> Indicators { get; set; }
    }
}