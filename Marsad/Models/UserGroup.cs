using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class UserGroup
    {
        public UserGroup()
        {
            this.Claims = new List<Claim>();
            this.ApplicationUsers = new List<ApplicationUser>();
        }

        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "برجاء إدخال إسم المجموعة")]
        [MaxLength(255,ErrorMessage ="إسم المجموعة يجب الا يتعدى 255 حرف")]
        [Display(Name="مجموعة المستخدمين")]
        public string Name { get; set; }

        public virtual List<ApplicationUser> ApplicationUsers { get; set; }
        public virtual List<Claim> Claims { get; set; }
    }
}