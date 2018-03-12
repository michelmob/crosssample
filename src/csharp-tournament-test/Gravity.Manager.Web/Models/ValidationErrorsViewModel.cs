using System.Collections.Generic;

namespace Gravity.Manager.Web.Models
{
    public class ValidationErrorsViewModel
    {
        public string Message { get; set; } 

        public ICollection<ValidationErrorViewModel> Errors { get; set; }
    }
}