using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.UserAccount.Models
{
    public class ChangeEmailRequestInputModel
    {
        //[Required]
        [EmailAddress]
        [DisplayName("New Email")]
        public string NewEmail { get; set; }
    }
}