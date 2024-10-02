using BankApp.Data;
using BankApp.Exceptions;
using BankApp.Models;
using BankApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services
{
    public class AccountService : IAccountService
    {
        private readonly IEntityRepository<Account> _entityRepository;
        private readonly Context _context;

        public AccountService(IEntityRepository<Account> entityRepository,Context context)
        {
            _entityRepository = entityRepository;
            _context = context;
        }
        public bool AddAccount(Account account)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {


                if (account == null)
                {
                    throw new InvalidAccountException("Invalid account details");
                }
                if (CheckValidity(account))
                {
                    var result = _entityRepository.Add(account);
                    _context.SaveChanges();
                    transaction.Commit();
                    return result;
                }
                transaction.Rollback();
                return false;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public bool DeleteAccount(Guid accountId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var getAccount = _entityRepository.GetById(accountId);
                if (getAccount == null || !getAccount.IsActive)
                {
                    throw new AccountNotFoundException("Account with this id not found or inactive.");
                }
                var result = _entityRepository.Delete(accountId);
                _context.SaveChanges();
                transaction.Commit();
                return result;
            }
            catch
            {
            transaction.Rollback(); throw; 
            }
        }
        public Account GetAccountById(Guid id)
        {
            var getAccount = _entityRepository.GetById(id);
            if(getAccount == null || !getAccount.IsActive)
            {
                throw new AccountNotFoundException("Account with this id not found :/");
            } 
            return getAccount;
        }

        public Account GetAccountByNumber(string accNo)
        {
            var getAccount = _entityRepository.GetAll().Where(acc => acc.AccountNumber == accNo && acc.IsActive).FirstOrDefault();
            if(getAccount == null || !getAccount.IsActive)
            {
                throw new AccountNotFoundException("Account with this account number not found :/");
            }
            return getAccount;
        }

        public List<Account> GetAllAccounts()
        {
            return _entityRepository.GetAll().Where(acc => acc.IsActive == true).ToList();
        }

        public Account UpdateAccount(Account account)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (account == null)
                {
                    throw new InvalidAccountException("Invalid Account");
                }
                var getAccount = GetAccountById(account.AccountId);
                if (getAccount == null || !getAccount.IsActive)
                {
                    throw new AccountNotFoundException("Account with this account number not found :/");
                }
                var updateAccount = _entityRepository.Update(account);
                _context.SaveChanges();
                transaction.Commit();
                return updateAccount;
            }
            catch
            {
                transaction.Rollback(); 
                throw;
            }
        }
        public bool CheckValidity(Account account)
        {
            foreach(var item in  _entityRepository.GetAll().ToList())
            {
                if(item.AccountNumber == account.AccountNumber)
                {
                    throw new AccountNumberAlreadyExistException("Account with this account number already exist");
                }
            }
            return true;
        }

        public List<Account> GetAccountByUserId(Guid userId)
        { 
            var getAccount = _entityRepository.GetAll().Where(a => a.UserId == userId && a.IsActive).ToList();
            if(getAccount == null)
            {
                throw new AccountNotFoundException("Account not found");
            }
            return getAccount;
        }

    }
}
