using BankApp.EnumFolder;

namespace BankApp.DTOs
{
    public class AccountSendDTO
    {
        public Guid AccountId { get; set; }
        public string AccountNumber { get; set; }
        public double AccountBalance { get; set; }
        public AccountType AccountType { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
