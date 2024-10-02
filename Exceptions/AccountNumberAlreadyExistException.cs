using System.Net;

namespace BankApp.Exceptions
{
    public class AccountNumberAlreadyExistException : Exception
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public AccountNumberAlreadyExistException(string message) : base(message)
        {
            StatusCode = (int)HttpStatusCode.BadRequest;
            Message = message;
        }
    }
}
