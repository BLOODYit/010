using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class GeoArea
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "رمز")]
        [Required(ErrorMessage ="برجاء إدخال الكود")]
        public string Code { get; set; }

        [Display(Name = "إسم")]
        [Required(ErrorMessage = "برجاء إدخال الإسم")]
        public string Name { get; set; }

        [Display(Name = "النطاق التابع")]     
        [RequiredNullable]
        public int? GeoAreaID { get; set; }
        public virtual GeoArea ParentGeoArea { get; set; }

        [Required]
        public string Type { get; set; }

        [NotMapped]
        public string ParentType
        {
            get
            {
                return GetParentName(Type);
            }
        }

        [NotMapped]
        public static Dictionary<string, string> Types = new Dictionary<string, string>() {
            { "District","الحي"},
            { "City","المدينة"},
            { "Town","البلدية"},
            { "Governorate","المحافظة"},
            { "SubRegion","الحاضرة"},
            { "Region","المنطقة"},
            {"Kingdom","المملكة" }
        };

        public static string GetParentName(string name)
        {            
            if (name.Equals("District"))
                return "City";
            else if (name.Equals("City"))
                return "Town";
            else if (name.Equals("Town"))
                return "Governorate";
            else if (name.Equals("Governorate"))
                return "SubRegion";
            else if (name.Equals("SubRegion"))
                return "Region";
            return "Kingdom";
        }

        public virtual List<GeoAreaValue> GeoAreaValues { get; set; }

    }

    internal class RequiredNullableAttribute : Attribute, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (String.IsNullOrWhiteSpace(validationContext.ObjectInstance.ToString()))
            {
                yield return new ValidationResult("برجاء إختيار ال" + validationContext.DisplayName);
            }
        }
    }
    

}