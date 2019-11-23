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
            this.Indicators = new List<Indicator>();
            this.Entities = new List<Entity>();
        }

        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "برجاء إدخال إسم القضية")]
        [MaxLength(255, ErrorMessage = "إسم القضية يجب الا يتعدى 255 حرف")]
        [Display(Name = "إسم القضية", ShortName = "القضية")]
        public string Name { get; set; }

        [Display(Name = "التعريف القضية")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }


        [Required(ErrorMessage = "برجاء إدخال سنة القضية")]
        [Display(Name = "سنة القضية", ShortName = "السنة")]
        public int Year { get; set; }


        [Display(Name = "فترة دراسة القضية")]
        public int? PeriodID { get; set; }
        public Period Period { get; set; }


        public virtual  List<CaseYear> CaseYears { get; set; }
        public virtual  List<Indicator> Indicators { get; set; }
        public virtual List<Entity> Entities { get; set; }

    }
}