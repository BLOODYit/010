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
            this.Users = new List<ApplicationUser>();
        }

        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "برجاء إدخال إسم الحزمة")]
        [MaxLength(255, ErrorMessage = "إسم الحزمة يجب الا يتعدى 255 حرف")]
        [Display(Name = "إسم الحزمة", ShortName = "الحزمة")]
        public string Name { get; set; }

        [Display(Name = "وصف الحزمة")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name="لون الحزمة")]
        public string Color { get; set; }

        public string GetColor()
        {
            if (string.IsNullOrWhiteSpace(Color))
                return "#000";
            return Color;
        }

        public virtual List<Indicator> Indicators { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}