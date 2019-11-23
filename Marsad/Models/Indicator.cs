using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marsad.Models
{
    public class Indicator
    {

        public Indicator()
        {
            this.CaseYearIndicators = new List<CaseYearIndicator>();
            this.IndicatorGroups = new List<IndicatorGroup>();
            this.Equations = new List<Equation>();
            this.Indicators = new List<Indicator>();
            this.Cases = new List<Case>();
            this.Users = new List<ApplicationUser>();
        }

        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "برجاء إدخال رمز المؤشر")]        
        [Display(Name = "رمز المؤشر")]
        [Remote("IsExist","Indicators",AdditionalFields ="ID", ErrorMessage ="رمز المؤشر يجب الا يتكرر")]
        [Range(1,int.MaxValue,ErrorMessage ="يجب ان يكون رمز المؤشر رقم موجب")]
        public int Code { get; set; }

        [Required(ErrorMessage = "برجاء إدخال إسم المؤشر")]
        [MaxLength(1024, ErrorMessage = "إسم المؤشر يجب الا يتعدى 1024 حرف")]
        [Display(Name = "إسم المؤشر")]
        public string Name { get; set; }

        [Required(ErrorMessage = "برجاء إدخال عدد العناصر")]
        [Range(0, int.MaxValue, ErrorMessage = "يجب ان يكون عدد العناصر رقم موجب")]
        [Display(Name = "عدد العناصر")]
        public int ElementCount { get; set; }

        [Display(Name = "وحدة القياس")]
        public string MeasureUnit { get; set; }

        [Display(Name ="نوع المؤشر")]
        public bool HasParent { get; set; }

        [Display(Name ="المؤشر الرئيسي")]
        public int? IndicatorID { get; set; }
        public Indicator ParentIndicator { get; set; }

        [Required(ErrorMessage ="برجاء أختيار تصنيف المؤشر")]
        [Display(Name="تصنيف المؤشر")]
        public int IndicatorTypeID { get; set; }
        public IndicatorType IndicatorType { get; set; }

        [Required(ErrorMessage ="برجاء إختيار الحزمة")]
        [Display(Name="الحزمة")]
        public int BundleID { get; set; }
        public virtual Bundle Bundle { get; set; }

        [Display(Name = "تعريف المؤشر")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "إرتباط المؤشر")]
        [DataType(DataType.MultilineText)]
        public string Correlation { get; set; }
        
        [Display(Name = "المرجعية")]
        [DataType(DataType.MultilineText)]
        public string References { get; set; }

        [Display(Name = "إسلوب حساب المؤشر")]
        [DataType(DataType.MultilineText)]
        public string CalculationMethod { get; set; }


        public List<CaseYearIndicator> CaseYearIndicators { get; set; }
        public List<IndicatorGroup> IndicatorGroups { get; set; }
        public List<Equation> Equations { get; set; }
        public List<Indicator> Indicators { get; set; }
        public List<Case> Cases { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}