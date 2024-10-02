using BankApp.Models;
using BankApp.Repository;

namespace BankApp.Services
{
    public interface IAccountService
    {
        public List<Account> GetAllAccounts();
        public Account GetAccountById(Guid id);
        public List<Account> GetAccountByUserId(Guid userId);
        public Account GetAccountByNumber(string accNo);
        public bool AddAccount(Account account);
        public Account UpdateAccount(Account account);
        public bool DeleteAccount(Guid accountId);
    }
}
