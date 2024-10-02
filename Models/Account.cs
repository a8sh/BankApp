using BankApp.EnumFolder;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApp.Models
{
    public class Account
    {
        [Key]
        public Guid AccountId { get; set; }

        [MaxLength(12, ErrorMessage = "Account number cannot exceed 12 characters.")]
        public string? AccountNumber { get; set; }

        [Required(ErrorMessage = "Account balance is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Account balance must be a positive value.")]
        public double AccountBalance { get; set; } = 0;

        [Required(ErrorMessage = "Account type is required.")]
        public AccountType AccountType { get; set; } // "Savings", "Current", "FD", etc.

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
        public Guid? UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
