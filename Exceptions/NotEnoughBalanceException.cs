using System.Net;

namespace BankApp.Exceptions
{
    public class NotEnoughBalanceException : Exception
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public NotEnoughBalanceException(string message) : base(message)
        {
            StatusCode = (int)HttpStatusCode.BadRequest;
            Message = message;
        }
    }
}
