using BankApp.Data;
using BankApp.EnumFolder;
using BankApp.Exceptions;
using BankApp.Models;
using BankApp.Repository;
using Microsoft.AspNetCore.Authentication;

namespace BankApp.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IEntityRepository<Transaction> _transactionRepository;
        private readonly IEntityRepository<Account> _accountRepository;
        private readonly Context _context;
        public TransactionService(IEntityRepository<Transaction> transactionRepository, IEntityRepository<Account> accountRepository,Context context)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _context = context;
        }
        public bool AddTransaction(Transaction transaction)
        {
            using var dbTransaction = _context.Database.BeginTransaction();
            try
            {


                var account = _accountRepository.GetAll().Where(a => a.AccountNumber == transaction.AccountFrom && a.IsActive).FirstOrDefault();
                if (account == null || !account.IsActive)
                {
                    throw new AccountNotFoundException("Account not found or inactive.");
                }
                if (transaction.AccountFrom == transaction.AccountTo)
                {
                    // If it's a self-transaction
                    transaction.AccountTo = account.AccountNumber;
                }
                transaction.TransactionDateTime = DateTime.UtcNow;
                if (transaction.TransactionType == TransactionType.credit)
                {
                    account.AccountBalance += transaction.Amount;
                }
                else if (transaction.TransactionType == TransactionType.debit)
                {
                    if (account.AccountBalance >= transaction.Amount)
                    {
                        account.AccountBalance -= transaction.Amount;
                    }
                    else
                    {
                        throw new NotEnoughBalanceException("Not enough balance.");
                    }
                }
                account.Transactions.Add(transaction);
                _transactionRepository.Add(transaction);
                _accountRepository.Update(account);

                _context.SaveChanges();
                dbTransaction.Commit();

                return true;
            }
            catch(NotEnoughBalanceException ex) 
            {
                dbTransaction.Rollback(); 
                throw ex;
            }
        }

        public List<Transaction> GetAllTransaction()
        {
            return _transactionRepository.GetAll().ToList();
        }

        public Transaction GetTransaction(Guid id)
        {
            var getTransaction = _transactionRepository.GetById(id);
            if(getTransaction == null)
            {
                throw new TransactionNotFoundException("Transaction with this id not found");
            }
            return getTransaction;
        }

        public List<Transaction> GetTransactionsByAccountNumber(string accountNumber)
        {
            var account = _accountRepository.GetAll().FirstOrDefault(a => a.AccountNumber == accountNumber && a.IsActive);

            if (account == null)
            {
                throw new AccountNotFoundException("Account not found or inactive.");
            }

            // Get all transactions associated with the found account
            var transactions = _transactionRepository.GetAll().Where(t => t.AccountId == account.AccountId).ToList();

            if (transactions == null || transactions.Count == 0)
            {
                throw new TransactionNotFoundException("No transactions found for this account.");
            }

            return transactions;
        }

        public bool TransferFunds(string sourceAccountNumber, string targetAccountNumber, double amount)
        {
            using var dbTransaction = _context.Database.BeginTransaction();
            try
            {


                var sourceAccount = _accountRepository.GetAll().Where(a => a.AccountNumber == sourceAccountNumber && a.IsActive).FirstOrDefault();
                var targetAccount = _accountRepository.GetAll().Where(a => a.AccountNumber == targetAccountNumber && a.IsActive).FirstOrDefault();

                if (sourceAccount == null || !sourceAccount.IsActive)
                {
                    throw new AccountNotFoundException("Sender's account not found");
                }
                if (targetAccount == null || !targetAccount.IsActive)
                {
                    throw new AccountNotFoundException("Reciever's account not found");
                }
                if (sourceAccount.AccountBalance < amount)
                {
                    throw new NotEnoughBalanceException("Insufficient balance");
                }

                var debitTransaction = new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    AccountFrom = sourceAccount.AccountNumber,
                    AccountTo = targetAccount.AccountNumber,
                    Amount = amount,
                    TransactionType = TransactionType.debit,
                    TransactionDateTime = DateTime.Now,
                    AccountId = sourceAccount.AccountId
                };
                sourceAccount.AccountBalance -= amount;
                sourceAccount.Transactions.Add(debitTransaction);
                _transactionRepository.Add(debitTransaction);

                var creditTransaction = new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    AccountFrom = sourceAccount.AccountNumber,
                    AccountTo = targetAccount.AccountNumber,
                    Amount = amount,
                    TransactionType = TransactionType.credit,
                    TransactionDateTime = DateTime.UtcNow,
                    AccountId = targetAccount.AccountId
                };
                targetAccount.AccountBalance += amount;
                targetAccount.Transactions.Add(creditTransaction);
                _transactionRepository.Add(creditTransaction);

                _accountRepository.Update(sourceAccount);
                _accountRepository.Update(targetAccount);

                _context.SaveChanges();
                dbTransaction.Commit();

                return true;
            }
            catch
            {
                dbTransaction.Rollback(); 
                return false;
            }
        }
        public List<Transaction> GetTransactionsByAccountNumberAndDateRange(string accountNumber, int months)
        {
            var account = _accountRepository.GetAll().FirstOrDefault(a => a.AccountNumber == accountNumber && a.IsActive);

            if (account == null)
            {
                throw new AccountNotFoundException("Account not found or inactive.");
            }

            DateTime startDate = DateTime.Now.AddMonths(-months);

            // Get transactions within the selected date range
            var transactions = _transactionRepository.GetAll()
                .Where(t => t.AccountId == account.AccountId && t.TransactionDateTime >= startDate)
                .ToList();

            if (transactions == null || transactions.Count == 0)
            {
                throw new TransactionNotFoundException("No transactions found for this account.");
            }

            return transactions;
        }

    }
}
