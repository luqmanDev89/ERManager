using System.ComponentModel.DataAnnotations;

namespace ERManager.ViewModels.User
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }  // Note: Removed Required attribute

        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}
