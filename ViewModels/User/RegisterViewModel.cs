using System.ComponentModel.DataAnnotations;

namespace ERManager.ViewModels.User
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "ناو")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "ئیمەیڵ")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "ووشەی نهێنی")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Role")]
        [Required]
        public string Role { get; set; }
    }
}

