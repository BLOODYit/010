using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models.ViewModels
{
    public class EntityViewModel
    {
        [Required(ErrorMessage = "يجب إدخال الجهة")]
        [MaxLength(255, ErrorMessage = "إسم الجهة يجب الا يتعدى 255 حرف")]
        [Display(Name = "الجهة")]
        public string Name { get; set; }

        [Display(Name = "الوصف")]
        public string Description { get; set; }
        
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة السر")]
        public string OfficerPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة السر")]
        [Compare("OfficerPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "إسم المستخدم")]        
        public string OfficerUserName { get; set; }
        [Display(Name = "إسم ضابط الإتصال")]
        public string OfficerName { get;  set; }
        [Display(Name = "بريد اليكتروني ضابط الإتصال")]
        public string OfficerEmail{ get; set; }
    }
}