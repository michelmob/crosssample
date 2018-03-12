using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gravity.Manager.Web.Tests
{
    public static class ModelValidator
    {
        public static IList<ValidationResult> Validate(object model)
        {
            var res = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            
            // Attribute-based.
            Validator.TryValidateObject(model, ctx, res, true);
            
            // Interface-based.
            res.AddRange((model as IValidatableObject)?.Validate(ctx));
            
            return res;
        }
    }
}