using System.Net;

namespace BankApp.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public UserNotFoundException(string message) : base(message)
        {
            StatusCode = (int)HttpStatusCode.BadRequest;
            Message = message;
        }
    }
}
