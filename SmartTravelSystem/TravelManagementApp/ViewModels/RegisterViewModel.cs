using System.ComponentModel.DataAnnotations;

namespace TravelManagementApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Customer Code is required")]
        [StringLength(30, ErrorMessage = "Code cannot exceed 30 characters")]
        [Display(Name = "Customer Code")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(150, ErrorMessage = "Full Name cannot exceed 150 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(200)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        public int? Age { get; set; }
    }
}
