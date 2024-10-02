using System.Net;

namespace BankApp.Exceptions
{
    public class InvalidAccountException : Exception
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public InvalidAccountException(string message) : base(message)
        {
            StatusCode = (int)HttpStatusCode.BadRequest;
            Message = message;
        }
    }
}
