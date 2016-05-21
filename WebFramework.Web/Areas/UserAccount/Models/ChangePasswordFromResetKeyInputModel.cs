using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web.Areas.UserAccount.Models
{
    public class ChangePasswordFromResetKeyInputModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password confirmation must match password.")]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
        
        [HiddenInput]
        public string Key { get; set; }
    }
}