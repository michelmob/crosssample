using System.ComponentModel.DataAnnotations;

namespace Gravity.Manager.Web.Models
{
    public class LogonViewModel
    {
        [MinLength(4)]
        [StringLength(100)]
        public string UserName { get; set; }

        [MinLength(6)]
        [StringLength(100)]
        public string Password { get; set; }

    }
}