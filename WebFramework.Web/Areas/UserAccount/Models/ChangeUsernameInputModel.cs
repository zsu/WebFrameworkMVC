using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.UserAccount.Models
{
    public class ChangeUsernameInputModel
    {
        [Required]
        [DisplayName("New Username")]
        public string NewUsername { get; set; }
    }
}