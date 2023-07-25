using System;

namespace IMS.BusinessRules.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException(string message)
           : base(message)
        {

        }
    }
}
