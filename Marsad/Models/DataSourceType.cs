using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class DataSourceType
    {
        public DataSourceType()
        {
            this.DataSources = new List<DataSource>();
        }

        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage ="برجاء إدخال نوع مصدر البيانات")]
        [MaxLength(255,ErrorMessage ="نوع مصدر البيانات يجب الا يتعدى 255 حرف")]
        [Display(Name = "نوع مصدر البيانات")]
        public string Name { get; set; }

        public List<DataSource> DataSources { get; set; }
    }
}