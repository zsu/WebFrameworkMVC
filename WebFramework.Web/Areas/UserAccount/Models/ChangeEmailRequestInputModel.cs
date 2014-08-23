using System.ComponentModel.DataAnnotations;

namespace Web.Areas.UserAccount.Models
{
    public class ChangeEmailRequestInputModel
    {
        //[Required]
        [EmailAddress]
        public string NewEmail { get; set; }
    }
}