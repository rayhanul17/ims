using System;

namespace IMS.BusinessRules.Exceptions
{
    public class DuplicateException : Exception
    {
        public DuplicateException(string message)
           : base(message)
        {

        }
    }
}
