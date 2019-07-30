using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class Period
    {

        public Period()
        {
            this.DataSources = new List<DataSource>();
        }

        [Key]
        public int ID { get; set; }
       
        [Required(ErrorMessage ="برجاء إدخال الفترة")]
        [Display(Name="الفترة")]
        [MaxLength(255)]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "برجاء إدخال السنة")]
        [Display(Name = "السنة")]
        [Range(0,int.MaxValue,ErrorMessage ="السنة يجب ان تكون أكبر من 0")]
        public int Year { get; set; }

        [Required(ErrorMessage = "برجاء إدخال الشهر")]
        [Display(Name = "الشهر")]
        [Range(0, int.MaxValue, ErrorMessage = "الشهر يجب ان تكون أكبر من 0")]
        public int Month { get; set; }

        [Required(ErrorMessage = "برجاء إدخال اليوم")]
        [Display(Name = "اليوم")]
        [Range(0, int.MaxValue, ErrorMessage = "اليوم يجب ان تكون أكبر من 0")]
        public int Day { get; set; }

        public List<DataSource> DataSources { get; set; }
    }
}