using System.Net;

namespace BankApp.Exceptions
{
    public class InvalidUserException : Exception
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public InvalidUserException(string message) : base(message)
        {
            StatusCode = (int)HttpStatusCode.BadRequest;
            Message = message;
        }
    }
}
