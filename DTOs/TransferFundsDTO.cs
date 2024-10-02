namespace BankApp.DTOs
{
    public class TransferFundsDTO
    {
        public double Amount { get; set; }
        public string? AccountFrom { get; set; }
        public string? AccountTo { get; set; }
    }
}
