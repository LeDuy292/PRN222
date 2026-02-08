using System.ComponentModel.DataAnnotations;

namespace TravelManagementApp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Code is required")]
        [Display(Name = "Customer Code")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
