using System.Net;

namespace BankApp.Exceptions
{
    public class TransactionNotFoundException : Exception
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public TransactionNotFoundException(string message) : base(message)
        {
            StatusCode = (int)HttpStatusCode.BadRequest;
            Message = message;
        }
    }
}
