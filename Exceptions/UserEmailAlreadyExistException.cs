using System.Net;

namespace BankApp.Exceptions
{
    public class UserEmailAlreadyExistException : Exception
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public UserEmailAlreadyExistException(string message) : base(message)
        {
            StatusCode = (int)HttpStatusCode.BadRequest;
            Message = message;
        }
    }
}
