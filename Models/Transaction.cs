using BankApp.EnumFolder;
using BankApp.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApp.Models
{
    public class Transaction
    {
        [Key]
        public Guid TransactionId { get; set; }

        [Required(ErrorMessage = "Transaction amount is required.")]
        public double Amount { get; set; }
        public DateTime? TransactionDateTime { get; set; } = DateTime.Now;
        [Required]
        public string? AccountFrom { get; set; }

        [Required]
        public string? AccountTo { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; } // "Credit" or "Debit"
        [Required]
        public Guid AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Account Account { get; set; }

    }
}
