using System.ComponentModel.DataAnnotations;

namespace ERManager.ViewModels.User
{
    public class CreateUserViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        // This list holds roles fetched from the database
        public List<string> Roles { get; set; }
    }
}
