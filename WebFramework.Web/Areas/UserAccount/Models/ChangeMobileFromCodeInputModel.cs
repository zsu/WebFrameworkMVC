using System.ComponentModel.DataAnnotations;

namespace Web.Areas.UserAccount.Models
{
    public class ChangeMobileFromCodeInputModel
    {
        [Required]
        public string Code { get; set; }
    }
    
}