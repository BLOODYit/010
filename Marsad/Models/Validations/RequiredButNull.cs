using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marsad.Models.Validations
{

    public class RequiredButNull : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string sErrorMessage = "برجاء إختيار {0}";
            if (string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(string.Format(sErrorMessage, string.IsNullOrWhiteSpace(validationContext.DisplayName) ? validationContext.DisplayName : validationContext.MemberName));
            }
            return ValidationResult.Success;
        }


        IEnumerable<ModelClientValidationRule> IClientValidatable.GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule mcvrTwo = new ModelClientValidationRule();
            string sErrorMessage = "برجاء إختيار {0}";
            mcvrTwo.ErrorMessage = string.Format(sErrorMessage, string.IsNullOrWhiteSpace(metadata.DisplayName) ? metadata.DisplayName : metadata.PropertyName);
            return new List<ModelClientValidationRule> {
                mcvrTwo
            };
        }
    }
}