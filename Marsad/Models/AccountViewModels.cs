using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Marsad.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [EmailAddress]
        [Display(Name = "البريد الاليكتروني")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة السر")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة السر")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "إسم المستخدم")]
        [Required]
        public string UserName { get; set; }

        [Display(Name = "الإسم")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "إسم الجهة")]
        public int? EntityID { get; set; }

        [Required]
        [Display(Name = "نوع المستخدم")]
        public string RoleID { get; set; }
    }


    public class EditUserViewModel
    {
        public string Id { get; set; }
        [EmailAddress]
        [Display(Name = "البريد الاليكتروني")]
        public string Email { get; set; }        
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة السر")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة السر")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "إسم المستخدم")]
        public string UserName { get; set; }
        [Display(Name = "الإسم")]
        public string Name { get; set; }
        [Display(Name = "إسم الجهة")]
        public int? EntityID { get; set; }
        [Display(Name = "نوع المستخدم")]
        public string RoleID { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public static class IdentityExtension
    {
        public static string GetName(this System.Security.Principal.IPrincipal user)
        {
            var nameClaim = ((ClaimsIdentity)user.Identity).FindFirst("Name");
            if (nameClaim != null)
                return nameClaim.Value;
            return "";
        }

        public static string GetImage(this System.Security.Principal.IPrincipal user)
        {
            var imageClaim = ((ClaimsIdentity)user.Identity).FindFirst("Image");
            if (imageClaim != null)
                return imageClaim.Value;
            return "";
        }
    }
}
