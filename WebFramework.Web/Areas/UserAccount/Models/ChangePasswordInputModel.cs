using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.UserAccount.Models
{
    public class ChangePasswordInputModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Old Password")]
        public string OldPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        [DisplayName("Confirm New Password")]
        public string NewPasswordConfirm { get; set; }
    }
}