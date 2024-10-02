using System.ComponentModel.DataAnnotations;

namespace BankApp.DTOs
{
    public class UserUpdateDto
    {
        [Key]
        public Guid UserId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? FilePath { get; set; }
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; }
        [MinLength(10, ErrorMessage = "Enter valid phone number")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Phone number should contain only digits.")]
        public string? PhoneNumber { get; set; }

        [MaxLength(100, ErrorMessage = "Address should not exceed 100 characters.")]
        public string? Address { get; set; }
    }
}
