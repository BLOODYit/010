using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class DataSourceGroup
    {
        public DataSourceGroup()
        {
            this.DataSources = new List<DataSource>();
        }

        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "برجاء إدخال الكود")]
        [Display(Name= "رمز المجموعة")]
        public int Code { get; set; }
        [Required(ErrorMessage ="برجاء إدخال الإسم")]
        [MaxLength(255,ErrorMessage ="يجب الا يتعدى الإسم 255 حرف")]
        [Display(Name = "مجموعة مصدر البيانات")]
        public string Name { get; set; }

        public List<DataSource> DataSources { get; set; }
    }
}