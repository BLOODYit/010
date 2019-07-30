using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class Case
    {
        public Case()
        {
            this.CaseYears = new List<CaseYear>();
        }

        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage ="برجاء إدخال إسم القضية")]
        [MaxLength(255,ErrorMessage ="إسم القضية يجب الا يتعدى 255 حرف")]
        [Display(Name ="إسم القضية",ShortName ="القضية")]
        public string Name { get; set; }

        [Required(ErrorMessage ="برجاء إدخال سنة القضية")]
        [Display(Name="سنة القضية",ShortName ="السنة")]
        public int Year { get; set; }

        [Display(Name ="وصف القضية")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public List<CaseYear> CaseYears{ get; set; }

    }
}