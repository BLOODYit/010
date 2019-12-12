using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        [Required(ErrorMessage = "برجاء إدخال رمز المجموعة")]
        [Display(Name= "رمز المجموعة")]
        [Remote("IsExist", "DataSourceGroups", AdditionalFields = "ID", ErrorMessage = "رمز المجموعة يجب الا يتكرر")]
        [Range(1, int.MaxValue, ErrorMessage = "يجب ان يكون رمز المجموعة رقم موجب")]
        public int Code { get; set; }
        [Required(ErrorMessage ="برجاء إدخال الإسم")]
        [MaxLength(255,ErrorMessage ="يجب الا يتعدى الإسم 255 حرف")]
        [Display(Name = "مجموعة مصدر البيانات")]
        public string Name { get; set; }

        public List<DataSource> DataSources { get; set; }
    }
}