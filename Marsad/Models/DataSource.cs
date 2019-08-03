using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class DataSource
    {
        public DataSource()
        {
            this.DataSourceGroups = new List<DataSourceGroup>();
            this.Elements = new List<Element>();
        }

        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage ="برجاء إخال رمز المصدر")]
        [Display(Name="رمز المصدر")]
        public int Code { get; set; }

        [Required(ErrorMessage = "برجاء إخال مصدر البيانات")]
        [MaxLength(255)]
        [Display(Name="مصدر البيانات")]
        public string Name { get; set; }

        [Required(ErrorMessage = "برجاء إخال تاريخ النشر")]
        [Display(Name = "تاريخ النشر")]
        public DateTime PublishDate { get; set; }

        [Required(ErrorMessage ="برجاء إختيار نوع التاريخ")]
        [Display(Name ="نوع التاريخ")]
        public bool IsHijri { get; set; }

        [Required(ErrorMessage ="برجاء إختيار نوع المصدر")]
        [Display(Name = "نوع المصدر")]
        public int DataSourceTypeID { get; set; }
        public DataSourceType DataSourceType { get; set; }

        [Display(Name = "رقم الطبعة")]
        [MaxLength(255)]
        public string PublishNumber { get; set; }

        [Display(Name = "الناشر")]
        public string PublisherName { get; set; }

        [Display(Name = "المؤلف")]
        public string AuthorName { get; set; }
        
        [Required]
        [Display(Name="الفترة")]
        public bool NoPeriod { get; set; }

        [Display(Name = "الفترة")]
        public int? PeriodID { get; set; }
        public Period Period { get; set; }

        [Display(Name="بدون جهة")]
        public bool NoEntity { get; set; }

        [Display(Name = "الجهة")]
        public int? EntityID { get; set; }
        public Entity Entity { get; set; }

        [Required]
        [Display(Name="عدد جديد / جزء")]
        public bool IsPart { get; set; }
          
        [NotMapped]
        public string PeriodicString
        {
            get
            {
                if (!NoPeriod)                
                    return "غير دوري";
                if (Period != null)
                    return Period.Name;
                return "";
            }
        }

        public List<DataSourceGroup> DataSourceGroups { get; set; }
        public List<Element> Elements { get; set; }
    }
}