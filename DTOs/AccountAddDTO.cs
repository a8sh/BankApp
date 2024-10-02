using BankApp.EnumFolder;
using System.ComponentModel.DataAnnotations;

namespace BankApp.DTOs
{
    public class AccountAddDTO
    {
        [Key]
        public Guid AccountId { get; set; }

        [Required(ErrorMessage = "Account number is required.")]
        [MaxLength(12, ErrorMessage = "Account number cannot exceed 12 characters.")]
        public string AccountNumber { get; set; }
        [Required]
        public AccountType AccountType { get; set; } // "Savings", "Current", "FD", etc.

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "UserId is required.")]
        public Guid UserId { get; set; }

    }
}
