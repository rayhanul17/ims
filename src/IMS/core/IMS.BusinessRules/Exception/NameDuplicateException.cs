using System;

namespace IMS.BusinessRules.Exceptions
{
    public class NameDuplicateException : Exception
    {
        public NameDuplicateException(string message)
           : base(message)
        {

        }
    }
}
