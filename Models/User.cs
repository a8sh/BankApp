using System;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        public string FilePath { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(25, ErrorMessage = "First name cannot exceed 25 characters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "The First name field should only contain alphabets.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(25, ErrorMessage = "Last name cannot exceed 25 characters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "The Last name field should only contain alphabets.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password should be at least 8 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [MinLength(10, ErrorMessage = "Enter valid phone number")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Phone number should contain only digits.")]
        public string PhoneNumber { get; set; }

        [MaxLength(100, ErrorMessage = "Address should not exceed 100 characters.")]
        public string Address { get; set; }

        public bool IsAdmin { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }
}
