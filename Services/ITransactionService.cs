using BankApp.Models;

namespace BankApp.Services
{
    public interface ITransactionService
    {
        public List<Transaction> GetAllTransaction();
        public Transaction GetTransaction(Guid id);
        public bool AddTransaction(Transaction transaction);
        public bool TransferFunds(string sourceAccountNumber, string targetAccountNumber, double amount);
        public List<Transaction> GetTransactionsByAccountNumber(string accountNumber);
        List<Transaction> GetTransactionsByAccountNumberAndDateRange(string accountNumber, int months);
    }
}
