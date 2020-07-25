using System;

namespace Application.Common.Exceptions
{
    public class IllegalOperationException : Exception
    {
        public IllegalOperationException(string message) : base(message)
        {
            
        }
    }
}