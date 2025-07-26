using System.ComponentModel.DataAnnotations;

namespace ERManager.ViewModels.User
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
