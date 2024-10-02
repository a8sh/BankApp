using BankApp.Models;

namespace BankApp.Services
{
    public interface IUserService
    {
        public List<User> GetAllUsers();
        public User GetUserById(Guid id);
        public User GetUserByName(string firstName);
        public User GetUserByEmail(string email);
        public bool AddUser (User user);
        public User UpdateUser (User user);
        public bool DeleteUser (Guid id);
        public User ActiveUser(Guid id);
    }
}
