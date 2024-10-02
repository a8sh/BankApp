using System.Net;

namespace BankApp.Exceptions
{
    public class UserPhoneNumberAlreadyExistException : Exception
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public UserPhoneNumberAlreadyExistException(string message) : base(message)
        {
            StatusCode = (int)HttpStatusCode.BadRequest;
            Message = message;
        }
    }
}
