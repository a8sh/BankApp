using BankApp.EnumFolder;

namespace BankApp.DTOs
{
    public class TransactionSendDTO
    {
        public Guid TransactionId { get; set; }
        public double Amount { get; set; }
        public TransactionType TransactionType { get; set; } // Assuming you have an enum TransactionType
        public string? AccountFrom { get; set; } 
        public string? AccountTo { get; set; }   
        public DateTime TransactionDateTime { get; set; }
    }
}
