using System.Net;

namespace BankApp.Exceptions
{
    public class AccountNotFoundException : Exception
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public AccountNotFoundException(string message) : base(message)
        {
            StatusCode = (int)HttpStatusCode.BadRequest;
            Message = message;
        }
    }
}
