using BankApp.Data;
using BankApp.Exceptions;
using BankApp.Models;
using BankApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services
{
    public class UserService : IUserService
    {
        private readonly IEntityRepository<User> _repository;
        private readonly Context _context;
        public UserService(IEntityRepository<User> repository,Context context)
        {
            _repository = repository;
            _context = context;
        }
        public bool AddUser(User user)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (user == null)
                {
                    return false;
                }
                if (CheckValidity(user))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    var result = _repository.Add(user);
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

        public bool DeleteUser(Guid id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var result = _repository.Delete(id);
                _context.SaveChanges();
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<User> GetAllUsers()
        {
            return _repository.GetAll().Where(user=>user.IsActive == true).ToList();
        }

        public User GetUserById(Guid id)
        {
            User getUer = _repository.GetById(id);
            if (getUer == null || !getUer.IsActive)
            {
                throw new  UserNotFoundException("User with this Id not found :/");
            }
            return getUer;
        }

        public User GetUserByName(string firstName)
        {
            User getUser = _repository.GetAll().Where(user=>user.FirstName == firstName).FirstOrDefault();
            if (getUser == null || !getUser.IsActive)
            {
                throw new UserNotFoundException($"{firstName} is not registered.");
            }
            return getUser;
        }

        public User UpdateUser(User user)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (user == null)
                {
                    throw new InvalidUserException("Invalid User");
                }
                var getUser = _repository.GetById(user.UserId);
                if (getUser == null || !getUser.IsActive)
                {
                    throw new UserNotFoundException("User not found");
                }
                var updateUser = _repository.Update(user);
                _context.SaveChanges();
                transaction.Commit();
                return updateUser;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public bool CheckValidity(User user)
        {
            foreach(var item in _repository.GetAll().ToList())
            {
                if(item.Email == user.Email)
                {
                    throw new UserEmailAlreadyExistException("Email already exist");
                }
                if(item.PhoneNumber == user.PhoneNumber)
                {
                    throw new UserPhoneNumberAlreadyExistException("Phone number already exist");
                }
            }
            return true;
        }

        public User GetUserByEmail(string email)
        {
            var getUser = _repository.GetAll().Where(x=>x.Email == email).FirstOrDefault();
            if(getUser == null || !getUser.IsActive)
            {
                throw new UserNotFoundException("User with this email id not found");
            }
            return getUser;
        }
        public User ActiveUser(Guid id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {

                var getUser = _repository.GetAll().Where(a => a.UserId == id && !a.IsActive).FirstOrDefault();
                if (getUser == null)
                {
                    throw new UserNotFoundException("User not found");
                }
                getUser.IsActive = true;
                _context.SaveChanges();
                transaction.Commit();
                return getUser;
            }
            catch (AccountNotFoundException ex)
            {
                transaction.Rollback();
                throw ex;
            }
            
        }
    }
}
