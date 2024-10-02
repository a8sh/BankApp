using BankApp.EnumFolder;

namespace BankApp.DTOs
{
    public class AccountUpdateDTO
    {
        public Guid AccountId { get; set; }
        public double AccountBalance { get; set; }
        public string? AccountNumber { get; set; }
        public Guid? UserId { get; set; }
    }
}
