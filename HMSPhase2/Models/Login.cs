
using System.ComponentModel.DataAnnotations;

namespace HMSPhase2.Models
{
    public class Login
    {
        [Required]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [Display(Name = "Remember on this computer")]
        public bool RememberMe { get; set; }
    }
}