using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marsad.Models
{
    public class Element
    {
        public Element()
        {
            this.EquationElements = new List<EquationElement>();
        }

        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage ="يجب إدخال رمز العنصر")]        
        [Display(Name="رمز العنصر")]
        [Remote("IsExist", "Elemets", AdditionalFields = "ID", ErrorMessage = "رمز العنصر يجب الا يتكرر")]
        [Range(1, int.MaxValue, ErrorMessage = "يجب ان يكون رمز العنصر رقم موجب")]
        public int Code { get; set; }

        [Required(ErrorMessage ="يجب إدخال العنصر")]
        [MaxLength(255, ErrorMessage = "إسم العنصر يجب الا يتعدى 255 حرف")]
        [Display(Name ="العنصر")]
        public string Name { get; set; }

        [MaxLength(255, ErrorMessage = "وحدة القياس يجب الا تتعدى 255 حرف")]
        [Display(Name = "وحدة القياس")]
        public string MeasureUnit { get; set; }

        [Required(ErrorMessage ="يجب إختيار مصدر البيانات")]
        [Display(Name = "مصدر البيانات")]
        public int DataSourceID { get; set; }
        public DataSource DataSource { get; set; }

        public List<EquationElement> EquationElements { get; set; }
    }
}