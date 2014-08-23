using System.ComponentModel.DataAnnotations;

namespace Web.Areas.UserAccount.Models
{
    public class SendUsernameReminderInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}