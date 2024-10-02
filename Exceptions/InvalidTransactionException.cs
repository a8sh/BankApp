﻿using System.Net;

namespace BankApp.Exceptions
{
    public class InvalidTransactionException : Exception
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public InvalidTransactionException(string message) : base(message)
        {
            StatusCode = (int)HttpStatusCode.BadRequest;
            Message = message;
        }
    }
}
